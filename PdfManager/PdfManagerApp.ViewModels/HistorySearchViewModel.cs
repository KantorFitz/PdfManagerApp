using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using PdfManagerApp.Domain.Entities;
using PdfManagerApp.Infrastructure;
using PdfManagerApp.ViewModels.Business.Enums;

namespace PdfManagerApp.ViewModels;

public class HistorySearchViewModel(DatabaseContext context) : BaseViewModel
{
    private int _booksHandled;

    private List<HistoricalBookDetail> _historicalBookDetails = new();
    private SearchFinishReason _searchFinishReason;
    private string _seekedPhrasesList;
    private SearchLog _selectedSearchLog;

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
            context.SearchLogs
                .OrderByDescending(x => x.CreatedAt)
                .Include(x => x.HistoricalFolders)
                .ThenInclude(x => x.HistoricalBookDetails)
                .ThenInclude(x => x.SearchResults.OrderBy(y => y.FoundOnPage))
                .Load();

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
}
