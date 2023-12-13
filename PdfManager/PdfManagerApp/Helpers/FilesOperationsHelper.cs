using System.IO;
using Microsoft.Win32;
using PdfManagerApp.Domain.Entities;
using PdfDocument = UglyToad.PdfPig.PdfDocument;

namespace PdfManagerApp.Helpers;

public static class FilesOperationsHelper
{
    public static IEnumerable<string> GetFolderPdfs(string path, byte scanLevel = 0)
    {
        var dirInfo = new DirectoryInfo(path);

        if (!dirInfo.Exists)
            return new List<string>();

        var pdfs = dirInfo.EnumerateFiles("*.pdf", new EnumerationOptions
            {
                RecurseSubdirectories = true,
                MaxRecursionDepth = scanLevel,
            })
            .AsParallel() 
            .Select(x => x.FullName);

        return pdfs;
    }
    
    public static IDictionary<string, List<string>> GetFolderPdfsDepth(string path)
    {
        var dirInfo = new DirectoryInfo(path);

        if (!dirInfo.Exists)
            return new Dictionary<string, List<string>>();

        var pdfs = dirInfo.EnumerateFiles("*.pdf", new EnumerationOptions
            {
                RecurseSubdirectories = true,
                MaxRecursionDepth = int.MaxValue,
            })
            .AsParallel()
            .GroupBy(x => x.DirectoryName)
            .ToDictionary(x => x.Key!, x => x.Select(y => y.FullName).ToList());

        return pdfs;
    }

    public static string ShowSaveFileDialog(string title, string defaultExtension, params string[] filters)
    {
        var filter = string.Join("|", filters);

        var saveFileDialog = new SaveFileDialog
        {
            Title = title,
            Filter = filter,
            DefaultExt = defaultExtension,
            AddExtension = true,
        };

        saveFileDialog.ShowDialog();

        return saveFileDialog.FileName;
    }

    public static async Task<BookDetail> GetBookDetailedEntityAsync(string path, Guid folderId)
    {
        return await Task.Run(() =>
        {
            PdfDocument? pdf = null;
            try
            {
                pdf = PdfDocument.Open(path);

                return new BookDetail
                {
                    FolderId = folderId,
                    FileName = path,
                    NumberOfPages = pdf.NumberOfPages,
                    Title = Path.GetFileName(path)
                };
            }
            catch (Exception)
            {
                return new BookDetail
                {
                    FolderId = folderId,
                    FileName = path,
                    Title = Path.GetFileName(path)
                };
            }
            finally
            {
                pdf?.Dispose();
            }
        });
    }
}