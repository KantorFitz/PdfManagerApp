﻿using System.Collections.ObjectModel;
using PdfManagerApp.ViewModels.Business.Models;

namespace PdfManagerApp.ViewModels;

public class MainWindowViewModel : BaseViewModel
{
    private string _chosenFolderPath = "Aktualna ścieżka";
    private int _currentFileCompleted;
    private int _currentFileCompletedMaximum = 1;
    private string _currentFileWorkLabel = "Current file";
    private int _filesCompleted;
    private int _filesCompletedMaximum = 1;
    private ObservableCollection<TextOccurenceModel> _foundOccurrences = new();
    private bool _isStartTextSearchingButtonVisible = true;
    private string _pdfAmountValue = "...";
    private string _searchText;

    public ObservableCollection<TextOccurenceModel> FoundOccurrences
    {
        get => _foundOccurrences;
        set => SetField(ref _foundOccurrences, value);
    }

    public bool IsStartTextSearchingButtonVisible
    {
        get => _isStartTextSearchingButtonVisible;
        set => SetField(ref _isStartTextSearchingButtonVisible, value);
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
}
