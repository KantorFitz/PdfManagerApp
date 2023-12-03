using System.Collections.ObjectModel;

namespace PdfManagerApp.Domain.Entities;

public class HistoricalFolder
{
    public Guid Id { get; set; }

    public string AbsolutePath { get; set; }


    #region Navigation properties

    public Guid SearchLogId { get; set; }
    public virtual SearchLog SearchLog { get; set; }

    public virtual ICollection<HistoricalBookDetail> HistoricalBookDetails { get; set; } = new ObservableCollection<HistoricalBookDetail>();

    #endregion
}
