using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using CsvHelper;
using CsvHelper.Configuration;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PdfManagerApp.Data;
using PdfManagerApp.Domain.Entities;
using PdfManagerApp.Helpers;
using PdfManagerApp.Infrastructure;
using PdfManagerApp.ViewModels;
using Path = System.IO.Path;

namespace PdfManagerApp.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private CancellationTokenSource _cts;

    private bool _isSearching;
    private readonly MainWindowViewModel _viewModel;
    private readonly SettingsWindowViewModel _settingsWindowViewModel;
    private readonly DatabaseContext _context;
    private readonly IServiceProvider _sp;
    private SearchLog _searchLog;

    public MainWindow(MainWindowViewModel viewModel, SettingsWindowViewModel settingsWindowViewModel, IServiceProvider sp, DatabaseContext context)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _settingsWindowViewModel = settingsWindowViewModel;
        _context = context;
        _sp = sp;
        DataContext = _viewModel;
    }

    protected override async void OnClosing(CancelEventArgs e)
    {
        if (_isSearching)
        {
            await _cts.CancelAsync();
            _searchLog.SearchFinishReason = (int)SearchFinishReason.OperationCancelled;
            await _context.SaveChangesAsync();
        }

        base.OnClosing(e);
    }

    private async void BtnStartSearch_OnClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SearchText.IsNullOrEmpty() || _viewModel.SearchText.IsNullOrWhiteSpace())
            return;

        if (_isSearching)
            return;

        _viewModel.StartTextSearchingButton = Visibility.Hidden;

        _cts?.Dispose();
        _cts = new();

        _isSearching = true;

        var textToSearch = _viewModel.SearchText;

        if (textToSearch.IsNullOrEmpty())
            return;

        _searchLog = new SearchLog
        {
            CreatedAt = DateTime.UtcNow,
            SeekedPhrasesJsonList = JsonConvert.SerializeObject(new List<string>{textToSearch}),
            SearchFinishReason = (int)SearchFinishReason.Unknown
        };
        await _context.SearchLogs.AddAsync(_searchLog);

        _viewModel.FilesCompleted = 0;
        _viewModel.FilesCompletedMaximum = _settingsWindowViewModel.TotalPdfCount();
        
        _viewModel.FoundOccurrences.Clear();

        try
        {
            foreach (var folder in _settingsWindowViewModel.Folders.TakeWhile(_ => !_cts.IsCancellationRequested))
            {
                var historicalFolder = new HistoricalFolder
                {
                    SearchLogId = _searchLog.Id,
                    AbsolutePath = folder.AbsolutePath,
                };
                await _context.HistoricalFolders.AddAsync(historicalFolder);

                foreach (var bookDetail in folder.BookDetails.TakeWhile(_ => !_cts.IsCancellationRequested))
                {
                    var historicalBookDetail = new HistoricalBookDetail
                    {
                        HistoricalFolderId = historicalFolder.Id,
                        NumberOfPages = bookDetail.NumberOfPages,
                        FileNameWithExtension = bookDetail.Title,
                    };
                    await _context.HistoricalBookDetails.AddAsync(historicalBookDetail);
                    
                    await ProcessPdfFile(bookDetail.FileName, textToSearch, historicalBookDetail.Id);
                }
            }
            
            _searchLog.SearchFinishReason = (int)SearchFinishReason.FinishedNormally;
        }
        catch (Exception exception)
        {
            await _cts.CancelAsync();
            _searchLog.SearchFinishReason = (int)SearchFinishReason.ExceptionOccured;
            MessageBox.Show(exception.Message, "Error occured");
        }
        finally
        {
            if (_cts.IsCancellationRequested)
                _searchLog.SearchFinishReason = (int)SearchFinishReason.OperationCancelled;

            await _context.SaveChangesAsync();
            _isSearching = false;
            _viewModel.StartTextSearchingButton = Visibility.Visible;
        }
    }

    private async Task ProcessPdfFile(string pdfPath, string textToSearch, Guid historicalBookDetailId)
    {
        if (_cts.Token.IsCancellationRequested)
            return;

        await Task.Run(async () =>
        {
            using var pdf = new PdfReader(pdfPath);

            _viewModel.CurrentFileCompletedMaximum = pdf.NumberOfPages;
            _viewModel.CurrentFileCompleted = 1;

            var fileName = Path.GetFileName(pdfPath);

            _viewModel.CurrentFileWorkLabel = $"{fileName}  |  {pdf.NumberOfPages} p";

            foreach (var pageNumber in Enumerable.Range(1, pdf.NumberOfPages).TakeWhile(_ => !_cts.IsCancellationRequested))
            {
                await SearchPdfPage(pdf, pageNumber, textToSearch, fileName, historicalBookDetailId);

                _viewModel.CurrentFileCompleted = pageNumber;
            }

            _viewModel.FilesCompleted++;
        });
    }

    private async Task SearchPdfPage(PdfReader pdf, int pageNumber, string textToSearch, string fileName, Guid historicalBookDetailId)
    {
        if (_cts.Token.IsCancellationRequested)
            return;
        
        var extractedText = PdfTextExtractor.GetTextFromPage(pdf, pageNumber);

        var query = extractedText
            .Split(new[] { ".", "\n", Environment.NewLine }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Where(sentence => sentence.Contains(textToSearch))
            .Select(x => new TextOccurenceModel
            {
                BookName = fileName,
                FoundOnPage = pageNumber,
                Sentence = x.Trim()
            })
            .AsParallel();

            foreach (var value in query.TakeWhile(_ => !_cts.IsCancellationRequested))
            {
                var searchResult = new SearchResult
                {
                    HistoricalBookDetailId = historicalBookDetailId,
                    FoundOnPage = value.FoundOnPage,
                    Sentence = value.Sentence
                };

                await _context.SearchResults.AddAsync(searchResult);
                
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _viewModel.FoundOccurrences.Add(value);
                });
            }
    }

    private async void BtnCancelSearch_OnClick(object sender, RoutedEventArgs e)
    {
        await _cts.CancelAsync();

        _isSearching = false;
        
        _viewModel.StartTextSearchingButton = Visibility.Visible;
    }

    private void BtnExportToCsv_OnClick(object sender, RoutedEventArgs e)
    {
        if (dgrFoundOccurrences.Items.IsEmpty)
            return;

        if (_isSearching)
            return;

        var savePath = FilesOperationsHelper.ShowSaveFileDialog("Zapis do pliku CSV", ".csv", "Plik CSV (*.csv)|*.csv");

        if (savePath.IsNullOrEmpty())
            return;

        var dataToSave = dgrFoundOccurrences.Items.Cast<TextOccurenceModel>().ToList();

        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            Encoding = new UTF8Encoding(true)
        };

        using var writer = new StreamWriter(savePath);
        using var csv = new CsvWriter(writer, csvConfig);

        csv.Context.RegisterClassMap<TextOccurenceModelMap>();

        csv.WriteHeader<TextOccurenceModel>();
        csv.NextRecord();
        csv.WriteRecords(dataToSave);
    }

    private void ShowSettingsButton_OnClick(object sender, RoutedEventArgs e)
    {
        using var scope = _sp.CreateScope();
        var settingsWindow = scope.ServiceProvider.GetRequiredService<SettingsWindow>();

        settingsWindow.Owner = this;

        settingsWindow.ShowDialog();
    }

    private void ShowHistorySearchButton_OnClick(object sender, RoutedEventArgs e)
    {
        using var scope = _sp.CreateScope();
        var settingsWindow = scope.ServiceProvider.GetRequiredService<HistorySearchWindow>();

        settingsWindow.Owner = this;

        settingsWindow.ShowDialog();
    }
}
