using System;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PDFManagerApp;

public static class Program
{
    public static void Main(string[] args)
    {
        var pdf = new PdfReader(@"PDFs\design-patterns-c-real-world-examples-2nd.pdf");

        var extractor = PdfTextExtractor.GetTextFromPage(pdf, 30);

        var d = pdf.GetPageN(10);
        
    }
}