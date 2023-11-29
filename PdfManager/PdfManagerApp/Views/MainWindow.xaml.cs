using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using CsvHelper;
using CsvHelper.Configuration;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Win32;
using PdfManagerApp.Models;
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
    private readonly MainWindowViewModel _viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }

    private void FolderPickerButton_OnClick(object sender, RoutedEventArgs e)
    {
        var folderPicker = new OpenFolderDialog
        {
            Title = "Wybierz folder zawierający pliki PDF",
        };

        folderPicker.ShowDialog(this);

        if (!folderPicker.FolderName.IsNullOrEmpty())
        {
            _viewModel.ChosenFolderPath = folderPicker.FolderName;
        }

        _viewModel.PdfAmountValue = GetFolderPdfs(_viewModel.ChosenFolderPath).Count().ToString();
    }

    private void AddToFolderList_OnClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel.ChosenFolderPath.IsNullOrEmpty())
            return;

        if (!Directory.Exists(_viewModel.ChosenFolderPath))
            return;

        if (lboAddedFolders.Items.Contains(_viewModel.ChosenFolderPath))
            return;

        lboAddedFolders.Items.Add(_viewModel.ChosenFolderPath);

        UpdatePdfList();
    }

    private static IEnumerable<string> GetFolderPdfs(string path, byte scanLevel = 0)
    {
        var dirInfo = new DirectoryInfo(path);

        if (!dirInfo.Exists)
            return new List<string>();

        var pdfs = dirInfo.EnumerateFiles("*.pdf", new EnumerationOptions
            {
                RecurseSubdirectories = true,
                MaxRecursionDepth = scanLevel,
            })
            .AsParallel()
            .Select(x => x.FullName);

        return pdfs;
    }

    private void UpdatePdfList()
    {
        var obtainedPdfs = GetFolderPdfs(_viewModel.ChosenFolderPath, 0);

        foreach (var obtainedPdf in obtainedPdfs.Where(obtainedPdf => !lboAddedPdfs.Items.Contains(obtainedPdf)))
        {
            lboAddedPdfs.Items.Add(obtainedPdf);
        }

        _viewModel.FilesCompletedMaximum = lboAddedPdfs.Items.Count;
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

        _viewModel.FilesCompleted = 0;
        _viewModel.FoundOccurrences.Clear();

        var pdfPaths = lboAddedPdfs.Items.Cast<string>().ToList();

        try
        {
            foreach (var pdfPath in pdfPaths.TakeWhile(_ => !_cts.IsCancellationRequested))
            {
                await Task.Run(() => ProcessPdfFile(pdfPath, textToSearch), _cts.Token);
            }
        }
        catch (Exception exception)
        {
            await _cts.CancelAsync();
            MessageBox.Show(exception.Message, "Error occured");
        }
        finally
        {
            _isSearching = false;
            _viewModel.StartTextSearchingButton = Visibility.Visible;
        }
    }

    private async Task ProcessPdfFile(string pdfPath, string textToSearch)
    {
        if (_cts.Token.IsCancellationRequested)
            return;

        using var pdf = new PdfReader(pdfPath);

        _viewModel.CurrentFileCompletedMaximum = pdf.NumberOfPages;
        _viewModel.CurrentFileCompleted = 1;

        var fileName = Path.GetFileName(pdfPath);
        
        _viewModel.CurrentFileWorkLabel = $"{fileName}  |  {pdf.NumberOfPages} p";

        var handledPage = 1;
        while (!_cts.IsCancellationRequested && handledPage < pdf.NumberOfPages)
        {
            await SearchPdfPage(pdf, handledPage, textToSearch, fileName);
            handledPage++;

            _viewModel.CurrentFileCompleted++;
        }

        _viewModel.FilesCompleted++;
    }

    private Task SearchPdfPage(PdfReader pdf, int pageNumber, string textToSearch, string fileName)
    {
        if (_cts.Token.IsCancellationRequested)
            return Task.CompletedTask;
        
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

        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            foreach (var value in query.TakeWhile(_ => !_cts.IsCancellationRequested))
            {
                _viewModel.FoundOccurrences.Add(value);
            }
        });
        
        return Task.CompletedTask;
    }

    private void BtnCancelSearch_OnClick(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
        _isSearching = false;
        
        _viewModel.StartTextSearchingButton = Visibility.Visible;
    }

    private void BtnExportToCsv_OnClick(object sender, RoutedEventArgs e)
    {
        if (dgrFoundOccurrences.Items.IsEmpty)
            return;

        if (_isSearching)
            return;

        var saveFileDialog = new SaveFileDialog
        {
            Title = "Zapis do pliku CSV",
            Filter = "Plik CSV (*.csv)|*.csv",
            DefaultExt = ".csv",
            AddExtension = true,
        };

        saveFileDialog.ShowDialog(this);

        var savePath = saveFileDialog.FileName;

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
}
