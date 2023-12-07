using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using PdfManagerApp.Data;
using PdfManagerApp.Domain.Entities;
using PdfManagerApp.Infrastructure;
using PdfManagerApp.Models;

namespace PdfManagerApp.ViewModels;

public class HistorySearchViewModel(DatabaseContext context) : HistorySearchWindowModel, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public SearchLog SelectedSearchLog
    {
        get => _selectedSearchLog;
        set => SetField(ref _selectedSearchLog, value);
    }

    public string SeekedPhrasesList
    {
        get => _seekedPhrasesList;
        set => SetField(ref _seekedPhrasesList, value);
    }

    public SearchFinishReason SearchFinishReason
    {
        get => _searchFinishReason;
        set => SetField(ref _searchFinishReason, value);
    }

    public ObservableCollection<SearchLog> SearchLogs
    {
        get
        {
            context.SearchLogs.Load();

            return context.SearchLogs.Local.ToObservableCollection();
        }
    }

    public int BooksHandled
    {
        get => _booksHandled;
        set => SetField(ref _booksHandled, value);
    }

    public List<HistoricalBookDetail> HistoricalBookDetails
    {
        get => _historicalBookDetails;
        set => SetField(ref _historicalBookDetails, value);
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