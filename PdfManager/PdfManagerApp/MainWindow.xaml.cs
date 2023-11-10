using System.IO;
using System.Windows;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Win32;
using PdfManagerApp.Models;
using Path = System.IO.Path;

namespace PdfManagerApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private CancellationTokenSource _cts;

    private bool isSearching;
    private List<TextOccurenceModel> _foundOccurrences = new();

    public MainWindow()
    {
        InitializeComponent();
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
            tbxChoosenFolderPath.Text = folderPicker.FolderName;
        }
        
        lblPdfAmountValue.Content = GetFolderPdfs(tbxChoosenFolderPath.Text).Count();
    }

    private void AddToFolderList_OnClick(object sender, RoutedEventArgs e)
    {
        if (tbxChoosenFolderPath.Text.IsNullOrEmpty())
            return;

        if (!Directory.Exists(tbxChoosenFolderPath.Text))
            return;

        if (lboAddedFolders.Items.Contains(tbxChoosenFolderPath.Text))
            return;

        lboAddedFolders.Items.Add(tbxChoosenFolderPath.Text);

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
        var obtainedPdfs = GetFolderPdfs(tbxChoosenFolderPath.Text, 0);

        foreach (var obtainedPdf in obtainedPdfs.Where(obtainedPdf => !lboAddedPdfs.Items.Contains(obtainedPdf)))
        {
            lboAddedPdfs.Items.Add(obtainedPdf);
        }

        pbFilesCompleted.Maximum = lboAddedPdfs.Items.Count;
    }

    private async void BtnStartSearch_OnClick(object sender, RoutedEventArgs e)
    {
        if (isSearching)
            return;
        
        _cts?.Dispose();
        _cts = new();

        isSearching = true;

        var textToSearch = tboSearchText.Text;

        if (textToSearch.IsNullOrEmpty())
            return;

        pbFilesCompleted.Value = 0;
        
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            dgrFoundOccurrences.Items.Clear();
        });

        var pdfPaths = lboAddedPdfs.Items.Cast<string>().ToList();

        try
        {
            foreach (var pdfPath in pdfPaths)
            {
                if (_cts.IsCancellationRequested)
                    break;

                await Task.Run(() => ProcessPdfFile(pdfPath, textToSearch), _cts.Token);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Error occured");
        }
    }

    private async Task ProcessPdfFile(string pdfPath, string textToSearch)
    {
        if (_cts.Token.IsCancellationRequested)
            return;

        using var pdf = new PdfReader(pdfPath);

        var fileName = Path.GetFileName(pdfPath);
        
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            CurrentFileWorkLabel.Content = $"{fileName}  |  {pdf.NumberOfPages} p";
        });

        var handledPage = 1;
        while (!_cts.IsCancellationRequested || handledPage >= pdf.NumberOfPages)
        {
            await SearchPdfPage(pdf, handledPage, textToSearch, fileName);
            handledPage++;
        }
        
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _foundOccurrences.ForEach(x => dgrFoundOccurrences.Items.Add(x));
            pbFilesCompleted.Value += 1;
        });
        
        _foundOccurrences.Clear();
    }

    private Task SearchPdfPage(PdfReader pdf, int pageNumber, string textToSearch, string fileName)
    {
        if (_cts.Token.IsCancellationRequested)
            return Task.CompletedTask;
        
        var extractedText = PdfTextExtractor.GetTextFromPage(pdf, pageNumber);

        _foundOccurrences.AddRange(
            extractedText
                .Split(new[] { ".", "\n", Environment.NewLine }, StringSplitOptions.None)
                .Where(sentence => sentence.Contains(textToSearch))
                .Select(x => new TextOccurenceModel
                {
                    BookName = fileName,
                    FoundOnPage = pageNumber,
                    Sentence = x.Trim()
                })
        );
        return Task.CompletedTask;
    }

    private void BtnCancelSearch_OnClick(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
        isSearching = false;
    }
}
