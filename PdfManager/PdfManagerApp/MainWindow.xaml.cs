using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace PdfManagerApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
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
            ChoosenFolderPath.Text = folderPicker.FolderName;
        }
    }

    private void AddToFolderList_OnClick(object sender, RoutedEventArgs e)
    {
        if (ChoosenFolderPath.Text.IsNullOrEmpty())
        {
            return;
        }

        if (!AddedFolders.Items.Contains(ChoosenFolderPath.Text))
        {
            AddedFolders.Items.Add(ChoosenFolderPath.Text);
        }
    }

    private void ScanFolder_OnClick(object sender, RoutedEventArgs e)
    {
        var foundPdfs = GetFolderPdfsCount(ChoosenFolderPath.Text, 1);

        if (foundPdfs < 0)
        {
            MessageBox.Show(this, "Nie znaleziono plików PDF.", "Skanowanie folderu", MessageBoxButton.OK);
            return;
        }

        MessageBox.Show(this, $"Znaleziono plików PDF: {foundPdfs}", "Skanowanie folderu", MessageBoxButton.OK);
        
        // MessageBox.Show()
    }

    /// <summary>
    /// 0 - scan all subfolders otherwise scan only up to given level <br /><br />
    /// 
    ///-1 when folder doesnt exist, <br />
    /// 0 or more otherwise
    /// </summary>
    private static int GetFolderPdfsCount(string path, byte scanLevel = 0)
    {
        var dirInfo = new DirectoryInfo(path);

        if (!dirInfo.Exists)
            return -1;

        var pdfCount = dirInfo.GetFiles("*.pdf", new EnumerationOptions
        {
            RecurseSubdirectories = true,
            MaxRecursionDepth = scanLevel
        });

        return pdfCount.Length;
    }
}

