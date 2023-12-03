﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PdfManagerApp.Data;
using PdfManagerApp.Domain.Entities;
using PdfManagerApp.Models;

namespace PdfManagerApp.ViewModels;

public class SettingsWindowViewModel : SettingsWindowModel, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public bool DepthSearchEnabled // TODO[2023-12-03 12:24:09]: change it to IsSearchEnabled
    {
        get => _depthSearchEnabled;
        set => SetField(ref _depthSearchEnabled, value);
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