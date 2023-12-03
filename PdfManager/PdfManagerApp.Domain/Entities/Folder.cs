using System.Collections.ObjectModel;

namespace PdfManagerApp.Domain.Entities;

public class Folder
{
    public Guid Id { get; set; }

    public string AbsolutePath { get; set; }
    public int PdfAmount { get; set; }

    #region Navigation properties

    public virtual ICollection<BookDetail> BookDetails { get; set; } = new ObservableCollection<BookDetail>();

    #endregion
}
