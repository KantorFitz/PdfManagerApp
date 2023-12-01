using System.ComponentModel;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using PdfManagerApp.Data;
using PdfManagerApp.Helpers;
using PdfManagerApp.ViewModels;

namespace PdfManagerApp.Views;

public partial class SettingsWindow : Window
{
    private SettingsWindowViewModel _viewModel;

    public SettingsWindow(SettingsWindowViewModel settingsMV)
    {
        InitializeComponent();
        _viewModel = settingsMV;
        DataContext = _viewModel;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    private void FolderPickerButton_OnClick(object sender, RoutedEventArgs e)
    {
        var folderPicker = new OpenFolderDialog
        {
            Title = "Wybierz folder zawierający pliki PDF",
        };

        folderPicker.ShowDialog(this);

        if (!folderPicker.FolderName.IsNullOrEmpty())
        {
            _viewModel.ChosenFolderPath = folderPicker.FolderName;
        }

        _viewModel.PdfAmountValue = FilesOperationsHelper.GetFolderPdfs(_viewModel.ChosenFolderPath).Count().ToString();
    }

    private void AddToFolderList_OnClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel.ChosenFolderPath.IsNullOrEmpty())
            return;

        if (!Directory.Exists(_viewModel.ChosenFolderPath))
            return;

        if (_viewModel.Folders.Any(x => x.FolderPath == _viewModel.ChosenFolderPath))
            return;

        UpdatePdfList(_viewModel.ChosenFolderPath);
    }

    private void UpdatePdfList(string path)
    {
        var obtainedPdfs = FilesOperationsHelper.GetFolderPdfs(path);

        Application.Current.Dispatcher.InvokeAsync(async () =>
        {
            var folderToAdd = new FolderWithBooksModel
            {
                FolderPath = _viewModel.ChosenFolderPath,
                BooksInFolder = new()
            };

            _viewModel.Folders.Add(folderToAdd);

            foreach (var obtainedPdf in obtainedPdfs)
            {
                folderToAdd.BooksInFolder.Add(await FilesOperationsHelper.GetBookDetailedInfo(obtainedPdf));
            }
        });
    }

    private async void BtnAddDepthFiles_OnClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel.ChosenFolderPath.IsNullOrEmpty())
            return;

        _viewModel.DepthSearchEnabled = false;
        await Task.Run(() => FillList(FilesOperationsHelper.GetFolderPdfsDepth(_viewModel.ChosenFolderPath)));
    }

    private Task FillList(IDictionary<string, List<string>> groupedPdfs)
    {
        _viewModel.TotalFoldersCount = groupedPdfs.Count;
        _viewModel.FoldersScanned = 0;
        foreach (var item in groupedPdfs)
        {
            if (_viewModel.Folders.Any(x => x.FolderPath == item.Key))
            {
                _viewModel.FoldersScanned++;
                continue;
            }

            var folderToAdd = new FolderWithBooksModel
            {
                FolderPath = item.Key,
                BooksInFolder = new()
            };

            Application.Current.Dispatcher.Invoke(async () =>
            {
                _viewModel.Folders.Add(folderToAdd);

                foreach (var pdfPath in item.Value)
                    folderToAdd.BooksInFolder.Add(await FilesOperationsHelper.GetBookDetailedInfo(pdfPath));
            });

            _viewModel.FoldersScanned++;
        }

        _viewModel.DepthSearchEnabled = true;
        return Task.CompletedTask;
    }
}