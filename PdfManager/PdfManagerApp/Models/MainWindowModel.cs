using System.Collections.ObjectModel;
using System.Windows;
using PdfManagerApp.Data;
using PdfManagerApp.Localization;

namespace PdfManagerApp.Models;

public class MainWindowModel
{
    protected string _chosenFolderPath = Locales.MainWindow_ActualFolderPath;
    protected string _pdfAmountValue = "...";
    protected string _searchText;
    protected int _filesCompleted;
    protected int _filesCompletedMaximum = 1;
    protected int _currentFileCompleted;
    protected int _currentFileCompletedMaximum = 1;
    protected string _currentFileWorkLabel = "Current file";
    protected Visibility _startTextSearchingButton;
    protected ObservableCollection<TextOccurenceModel> _foundOccurrences = new();
}