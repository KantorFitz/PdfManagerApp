using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PdfManagerApp.Models;

namespace PdfManagerApp.ViewModels;

public class MainWindowViewModel : MainWindowModel, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<TextOccurenceModel> FoundOccurrences
    {
        get => _foundOccurrences;
        set => SetField(ref _foundOccurrences, value);
    }

    public Visibility StartTextSearchingButton
    {
        get => _startTextSearchingButton;
        set => SetField(ref _startTextSearchingButton, value);
    }

    public int FilesCompletedMaximum
    {
        get => _filesCompletedMaximum;
        set => SetField(ref _filesCompletedMaximum, value);
    }

    public int CurrentFileCompletedMaximum
    {
        get => _currentFileCompletedMaximum;
        set => SetField(ref _currentFileCompletedMaximum, value);
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

    public string SearchText
    {
        get => _searchText;
        set => SetField(ref _searchText, value);
    }

    public int FilesCompleted
    {
        get => _filesCompleted;
        set => SetField(ref _filesCompleted, value);
    }

    public int CurrentFileCompleted
    {
        get => _currentFileCompleted;
        set => SetField(ref _currentFileCompleted, value);
    }

    public string CurrentFileWorkLabel
    {
        get => _currentFileWorkLabel;
        set => SetField(ref _currentFileWorkLabel, value);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}