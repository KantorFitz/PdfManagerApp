using System.Collections.ObjectModel;

namespace PdfManagerApp.Domain.Entities;

public class HistoricalBookDetail
{
    public Guid Id { get; set; }

    public string FileNameWithExtension { get; set; }
    public int NumberOfPages { get; set; }

    #region Navigation properties

    public Guid HistoricalFolderId { get; set; }
    public virtual HistoricalFolder HistoricalFolder { get; set; }

    public virtual ICollection<SearchResult> SearchResults { get; set; } = new ObservableCollection<SearchResult>();

    #endregion
}
