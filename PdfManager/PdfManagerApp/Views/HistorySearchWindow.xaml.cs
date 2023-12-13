using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using PdfManagerApp.Domain.Entities;
using PdfManagerApp.ViewModels;
using PdfManagerApp.ViewModels.Business.Enums;

namespace PdfManagerApp.Views;

public partial class HistorySearchWindow : Window
{
    private readonly HistorySearchViewModel _viewModel;

    public HistorySearchWindow(HistorySearchViewModel viewModel)
    {
        _viewModel = viewModel;
        InitializeComponent();

        DataContext = _viewModel;
    }

    private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var searchLog = _viewModel.SelectedSearchLog = e.AddedItems.Cast<SearchLog>().Single();
        
        _viewModel.HistoricalBookDetails = searchLog.HistoricalFolders.SelectMany(x => x.HistoricalBookDetails).ToList();

        _viewModel.SeekedPhrasesList = string.Join(", ", (JsonConvert.DeserializeObject<List<string>>(searchLog.SeekedPhrasesJsonList)));
        _viewModel.SearchFinishReason = (SearchFinishReason)searchLog.SearchFinishReason;
        _viewModel.BooksHandled = searchLog.HistoricalFolders.SelectMany(x => x.HistoricalBookDetails).Count();
    }
}
