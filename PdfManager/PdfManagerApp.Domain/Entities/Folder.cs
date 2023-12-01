namespace PdfManagerApp.Domain.Entities;

public class Folder
{
    public Guid Id { get; set; }

    public string AbsolutePath { get; set; }
    public int PdfAmount { get; set; }

    public virtual ICollection<BookDetail> BookDetails { get; set; }
}