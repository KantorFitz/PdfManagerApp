using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PdfManagerApp.Domain.Entities;

namespace PdfManagerApp.ViewModels;

public class SettingsWindowViewModel : BaseViewModel
{
    private bool _canPerformFilesSearch = true;

    private string _chosenFolderPath = "Aktualna ścieżka";
    private ObservableCollection<Folder> _folders = new();
    private int _foldersScanned;
    private string _pdfAmountValue = "...";
    private int _totalFoldersCount = 1;

    public bool CanPerformFilesSearch
    {
        get => _canPerformFilesSearch;
        set => SetField(ref _canPerformFilesSearch, value);
    }

    public int TotalFoldersCount
    {
        get => _totalFoldersCount;
        set => SetField(ref _totalFoldersCount, value);
    }

    public int FoldersScanned
    {
        get => _foldersScanned;
        set => SetField(ref _foldersScanned, value);
    }

    public string ChosenFolderPath
    {
        get => _chosenFolderPath;
        set => SetField(ref _chosenFolderPath, value);
    }

    public string PdfAmountValue
    {
        get => _pdfAmountValue;
        set => SetField(ref _pdfAmountValue, value);
    }

    public ObservableCollection<Folder> Folders
    {
        get => _folders;
        set => SetField(ref _folders, value);
    }

    public int TotalPdfCount()
    {
        return Folders.SelectMany(x => x.BookDetails).Count();
    }
}
