using MuPDFCore;
using PDFManagerApp.PDFs;
using UglyToad.PdfPig;

namespace PDFManagerApp;

public static class Program
{
    public static void Main(string[] args)
    {
        // MuPdfImageExtractionExample();
        PdfPigExample();
    }

    private static void PdfPigExample()
    {
        using var document = PdfDocument.Open(PdfHelper.GetSamplePdfPath());

        foreach (var page in document.GetPages())
        {
            var pageText = page.Text;

            foreach (var word in page.GetWords())
            {
                var pageWord = word.Text;
            }
            
            
        }
    }
    
    private static void MuPdfImageExtractionExample()
    {
        using var ctx = new MuPDFContext();

        using var document = new MuPDFDocument(ctx, PdfHelper.GetSamplePdfPath());

        var pageIndex = 2;

        var zoomLevel = 1;

        document.SaveImage(pageIndex, zoomLevel, PixelFormats.RGB, @"C:\Users\Kantor\Desktop\publish\output.png", RasterOutputFileTypes.PNG);
    }
}
