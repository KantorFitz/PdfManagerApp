using System.Collections.ObjectModel;

namespace PdfManagerApp.Data;

public class FolderWithBooksModel
{
    public string FolderPath { get; set; }
    public ObservableCollection<BookDetailInfo> BooksInFolder { get; set; }
}