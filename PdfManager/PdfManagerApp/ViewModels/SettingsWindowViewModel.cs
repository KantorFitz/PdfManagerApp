using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PdfManagerApp.Domain.Entities;
using PdfManagerApp.Models;

namespace PdfManagerApp.ViewModels;

public class SettingsWindowViewModel : SettingsWindowModel, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

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

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}