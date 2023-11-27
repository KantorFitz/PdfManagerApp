using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using iTextSharp.text;
using MuPDFCore;
using PDFManagerApp.PDFs;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Filters;

namespace PDFManagerApp;

public static class Program
{
    private const string outputFolder = "PdfOutputs";
    
    public static void Main(string[] args)
    {
        Directory.CreateDirectory(outputFolder);
        
        // MuPdfImageExtractionExample();
        PdfPigExampleImages();
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

    private static void PdfPigExampleImages()
    {
        using var document = PdfDocument.Open(PdfHelper.GetSamplePdfPathWithImages());

        foreach (var page in document.GetPages())
        {
            foreach (var image in page.GetImages())
            {
                var imageType = string.Empty;

                if (image.TryGetPng(out var byteArray))
                {
                    imageType = "png";
                }
                else if(image.TryGetBytes(out var ByteList))
                {
                    byteArray = ByteList.ToArray();
                    imageType = "most likely JPG";
                }
                else
                {
                    byteArray = image.RawBytes.ToArray();
                    imageType = "unknown";
                }

                File.WriteAllBytes(Path.Join(outputFolder, "test.png"), byteArray);

            }
            
        }
        
        byte[] ApplyFilters(IPdfImage Image)
        {
            byte[] Result = Image.RawBytes.ToArray();
            IReadOnlyList<IFilter> Filters = DefaultFilterProvider.Instance.GetFilters(Image.ImageDictionary);

            foreach (var Filter in Filters)
            {
                if (Filter.IsSupported)
                {
                    Result = Filter.Decode(Result, Image.ImageDictionary, 0);
                }
            }
            return Result;
        }
    }
    
    private static void MuPdfImageExtractionExample()
    {
        using var ctx = new MuPDFContext();

        using var document = new MuPDFDocument(ctx, PdfHelper.GetSamplePdfPath());

        var pageIndex = 2;

        var zoomLevel = 1;

        document.SaveImage(pageIndex, zoomLevel, PixelFormats.RGB, Path.Join(outputFolder, "test.png"), RasterOutputFileTypes.PNG);
    }
}
