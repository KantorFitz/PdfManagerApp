namespace PdfManagerApp.Domain.Entities;

public class SearchResult
{
    public Guid Id { get; set; }

    public int FoundOnPage { get; set; }
    public string Sentence { get; set; }

    #region Navigation properties

    public Guid HistoricalBookDetailId { get; set; }
    public virtual HistoricalBookDetail HistoricalBookDetail { get; set; }

    #endregion
}
