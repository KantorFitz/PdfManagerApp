using CsvHelper.Configuration;

namespace PdfManagerApp.Data;

public class TextOccurenceModel
{
    public string BookName { get; set; }
    public string Sentence { get; set; }   
    public int FoundOnPage { get; set; }
}

public sealed class TextOccurenceModelMap : ClassMap<TextOccurenceModel>
{
    public TextOccurenceModelMap()
    {
        Map(x => x.BookName).Index(0);
        Map(x => x.FoundOnPage).Index(1);
        Map(x => x.Sentence).Index(2);
    }
}
