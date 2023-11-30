using System.Collections.ObjectModel;
using System.Windows;
using PdfManagerApp.Data;
using PdfManagerApp.Localization;

namespace PdfManagerApp.Models;

public class SettingsWindowModel
{
    protected string _chosenFolderPath = Locales.MainWindow_ActualFolderPath;
    protected string _pdfAmountValue = "...";
    protected int _totalFoldersCount = 1;
    protected int _foldersScanned;
    protected bool _depthSearchEnabled = true;
    
    protected ObservableCollection<FolderWithBooksModel> _folders = new();
}