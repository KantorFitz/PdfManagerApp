using System.Collections.ObjectModel;
using PdfManagerApp.Domain.Entities;
using PdfManagerApp.Localization;

namespace PdfManagerApp.Models;

public class SettingsWindowModel
{
    protected string _chosenFolderPath = Locales.MainWindow_ActualFolderPath;
    protected string _pdfAmountValue = "...";
    protected int _totalFoldersCount = 1;
    protected int _foldersScanned;
    protected bool _depthSearchEnabled = true;
    
    protected ObservableCollection<Folder> _folders = new();
}