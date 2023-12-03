using System.Collections.ObjectModel;

namespace PdfManagerApp.Domain.Entities;

public class SearchLog
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }
    public int SearchFinishReason { get; set; } // TODO[2023-12-03 15:02:46]: add enum

    public string SeekedPhrasesJsonList { get; set; }

    #region Navigation properties

    public virtual ICollection<HistoricalFolder> HistoricalFolders { get; set; } = new ObservableCollection<HistoricalFolder>();

    #endregion
}
