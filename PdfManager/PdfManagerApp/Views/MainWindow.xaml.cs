using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using CsvHelper;
using CsvHelper.Configuration;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Extensions.DependencyInjection;
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

    public MainWindow(MainWindowViewModel viewModel, SettingsWindowViewModel settingsWindowViewModel, IServiceProvider sp, DatabaseContext context)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _settingsWindowViewModel = settingsWindowViewModel;
        _context = context;
        _sp = sp;
        DataContext = _viewModel;
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

        var searchLog = new SearchLog
        {
            CreatedAt = DateTime.UtcNow,
            SeekedPhrasesJsonList = textToSearch,
            SearchFinishReason = (int)SearchFinishReason.Unknown
        };
        await _context.SearchLogs.AddAsync(searchLog);

        _viewModel.FilesCompleted = 0;
        _viewModel.FoundOccurrences.Clear();

        try
        {
            foreach (var folder in _settingsWindowViewModel.Folders.TakeWhile(_ => !_cts.IsCancellationRequested))
            {
                var historicalFolder = new HistoricalFolder
                {
                    SearchLogId = searchLog.Id,
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
                    
                    await Task.Run(() => ProcessPdfFile(bookDetail.FileName, textToSearch, historicalBookDetail.Id));
                }
            }
            
            searchLog.SearchFinishReason = (int)SearchFinishReason.FinishedNormally;
        }
        catch (Exception exception)
        {
            await _cts.CancelAsync();
            searchLog.SearchFinishReason = (int)SearchFinishReason.ExceptionOccured;
            MessageBox.Show(exception.Message, "Error occured");
        }
        finally
        {
            if (_cts.IsCancellationRequested)
                searchLog.SearchFinishReason = (int)SearchFinishReason.OperationCancelled;

            await _context.SaveChangesAsync();
            _isSearching = false;
            _viewModel.StartTextSearchingButton = Visibility.Visible;
        }
    }

    private async Task ProcessPdfFile(string pdfPath, string textToSearch, Guid historicalBookDetailId)
    {
        if (_cts.Token.IsCancellationRequested)
            return;

        using var pdf = new PdfReader(pdfPath);

        _viewModel.CurrentFileCompletedMaximum = pdf.NumberOfPages;
        _viewModel.CurrentFileCompleted = 1;

        var fileName = Path.GetFileName(pdfPath);
        
        _viewModel.CurrentFileWorkLabel = $"{fileName}  |  {pdf.NumberOfPages} p";

        while (!_cts.IsCancellationRequested && _viewModel.CurrentFileCompleted < pdf.NumberOfPages)
        {
            await SearchPdfPage(pdf, _viewModel.CurrentFileCompleted, textToSearch, pdfPath, historicalBookDetailId);

            _viewModel.CurrentFileCompleted++;
        }

        _viewModel.FilesCompleted++;
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
}
