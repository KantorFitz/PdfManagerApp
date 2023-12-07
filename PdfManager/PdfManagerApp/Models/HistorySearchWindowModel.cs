using PdfManagerApp.Data;
using PdfManagerApp.Domain.Entities;

namespace PdfManagerApp.Models;

public class HistorySearchWindowModel
{
    protected List<HistoricalBookDetail> _historicalBookDetails = new();

    protected SearchLog _selectedSearchLog;
    protected SearchFinishReason _searchFinishReason;

    protected int _booksHandled;
    protected string _seekedPhrasesList;
}
