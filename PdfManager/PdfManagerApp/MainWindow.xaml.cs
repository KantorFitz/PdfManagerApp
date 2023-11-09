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
    private CancellationTokenSource _cts = new();

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
        if (_cts.Token.IsCancellationRequested)
        {
            _cts.Dispose();
            _cts = new();
            
            return;
        }

        var textToSearch = tboSearchText.Text;

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
                    continue;

                await Task.Run(() => ProcessPdfFile(pdfPath, textToSearch), _cts.Token).ConfigureAwait(false);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Error occured");
        }
        finally
        {
            _cts.Dispose();
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

        var tasks = Enumerable.Range(1, pdf.NumberOfPages).Select(x => SearchPdfPage(pdf, x, textToSearch)).ToList();

        var pages = await Task.WhenAll(tasks);

        var foundOccurrences = new List<TextOccurenceModel>();
        
        foreach (var result in pages)
        {
            foundOccurrences.AddRange(
                result
                    .First()
                    .Value
                    .Select(sentence => new TextOccurenceModel
                    {
                        BookName = fileName,
                        FoundOnPage = result.First().Key,
                        Sentence = sentence.Trim()
                    }));
        }

        await Application.Current.Dispatcher.InvokeAsync(() => { foundOccurrences.ForEach(x => dgrFoundOccurrences.Items.Add(x)); });

        await Application.Current.Dispatcher.InvokeAsync(() => pbFilesCompleted.Value += 1);
    }

    private async Task<Dictionary<int, List<string>>> SearchPdfPage(PdfReader pdf, int pageNumber, string textToSearch)
    {
        if (_cts.Token.IsCancellationRequested)
            return await Task.FromResult(new Dictionary<int, List<string>>
            {
                [pageNumber] = new()
            });
        
        var extractedText = PdfTextExtractor.GetTextFromPage(pdf, pageNumber);

        return await Task.FromResult(new Dictionary<int, List<string>>
        {
            [pageNumber] = extractedText.Split(new []{".", "\n", "\n\t"}, StringSplitOptions.None).Where(sentence => sentence.Contains(textToSearch)).ToList()
        });
    }

    private async void BtnCancelSearch_OnClick(object sender, RoutedEventArgs e)
    {
        await _cts.CancelAsync();
    }
}
