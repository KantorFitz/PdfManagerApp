using System.IO;
using Microsoft.Win32;

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
}