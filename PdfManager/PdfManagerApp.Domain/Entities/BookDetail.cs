﻿namespace PdfManagerApp.Domain.Entities;

public class BookDetail
{
    public Guid Id { get; set; }

    public string FileName { get; set; }
    public string Title { get; set; }
    public int NumberOfPages { get; set; }

    #region Navigation properties

    public Guid FolderId { get; set; }
    public virtual Folder Folder { get; set; }

    #endregion
}
