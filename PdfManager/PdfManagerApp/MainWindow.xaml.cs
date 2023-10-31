using System.IO;
using System.Windows;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Win32;
using Path = System.IO.Path;

namespace PdfManagerApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private CancellationTokenSource _cts = new();
    private bool _lockedObject = false;

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

    private void ScanFolder_OnClick(object sender, RoutedEventArgs e)
    {
        var foundPdfs = GetFolderPdfs(tbxChoosenFolderPath.Text).ToList();

        if (foundPdfs.Count == 0)
        {
            MessageBox.Show(this, "Nie znaleziono plików PDF.", "Skanowanie folderu", MessageBoxButton.OK);
            return;
        }

        MessageBox.Show(this, $"Znaleziono plików PDF: {foundPdfs.Count}", "Skanowanie folderu", MessageBoxButton.OK);

        // MessageBox.Show()
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
        if (_lockedObject)
            return;

        _lockedObject = true;
        _cts = new();

        var foundOccurences = new Dictionary<string, List<int>>();
        var textToSearch = tboSearchText.Text;

        pbFilesCompleted.Value = 0;

        foreach (var pdfPath in lboAddedPdfs.Items.Cast<string>())
        {
            if (_cts.IsCancellationRequested)
                continue;

            var pdfName = Path.GetFileName(pdfPath);

            await Task.Run(() =>
            {
                PdfReader? pdf = null;
                try
                {
                    pdf = new PdfReader(pdfPath);
                    var totalPagesCount = pdf.NumberOfPages;

                    for (var i = 1; i <= totalPagesCount; i++)
                    {
                        var extractedText = PdfTextExtractor.GetTextFromPage(pdf, i);

                        if (!extractedText.Contains(textToSearch))
                            continue;

                        if (foundOccurences.ContainsKey(pdfName))
                            foundOccurences[pdfName].Add(i);
                        else
                            foundOccurences[pdfName] = new List<int> { i };
                    }
                }
                finally
                {
                    pdf?.Dispose();
                }

                Application.Current.Dispatcher.Invoke(() => { pbFilesCompleted.Value += 1; });

            }, _cts.Token);
        }
    }

    private async void BtnCancelSearch_OnClick(object sender, RoutedEventArgs e)
    {
        await _cts.CancelAsync();
        _lockedObject = false;
    }
}