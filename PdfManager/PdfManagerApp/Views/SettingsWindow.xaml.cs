using System.ComponentModel;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using PdfManagerApp.Domain.Entities;
using PdfManagerApp.Helpers;
using PdfManagerApp.Infrastructure;
using PdfManagerApp.ViewModels;

namespace PdfManagerApp.Views;

public partial class SettingsWindow : Window
{
    private SettingsWindowViewModel _viewModel;
    private readonly DatabaseContext _dbContext;

    public SettingsWindow(SettingsWindowViewModel settingsMv, DatabaseContext dbContext)
    {
        InitializeComponent();
        _dbContext = dbContext;

        _viewModel = settingsMv;
        DataContext = _viewModel;
    }

    private async Task UpdateDatabase()
    {
        await _dbContext.Folders.ExecuteDeleteAsync();

        await _dbContext.AddRangeAsync(_viewModel.Folders);
        
        await _dbContext.SaveChangesAsync();
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
        // if (_viewModel.ChosenFolderPath.IsNullOrEmpty())
        //     return;
        //
        // if (!Directory.Exists(_viewModel.ChosenFolderPath))
        //     return;
        //
        // if (_viewModel.Folders.Any(x => x.FolderPath == _viewModel.ChosenFolderPath))
        //     return;
        //
        // UpdatePdfList(_viewModel.ChosenFolderPath);
    }

    // private void UpdatePdfList(string path)
    // {
    //     var obtainedPdfs = FilesOperationsHelper.GetFolderPdfs(path);
    //
    //     Application.Current.Dispatcher.InvokeAsync(async () =>
    //     {
    //         var folderToAdd = new Folder
    //         {
    //             AbsolutePath = _viewModel.ChosenFolderPath,
    //         };
    //
    //         _viewModel.Folders.Add(folderToAdd);
    //
    //         foreach (var obtainedPdf in obtainedPdfs)
    //         {
    //             folderToAdd.BooksInFolder.Add(await FilesOperationsHelper.GetBookDetailedInfo(obtainedPdf));
    //         }
    //     });
    // }

    private async void BtnAddDepthFiles_OnClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel.ChosenFolderPath.IsNullOrEmpty())
            return;

        _viewModel.DepthSearchEnabled = false;
        await Task.Run(() => FillList(FilesOperationsHelper.GetFolderPdfsDepth(_viewModel.ChosenFolderPath)));
    }

    private async Task FillList(IDictionary<string, List<string>> groupedPdfs)
    {
        _viewModel.TotalFoldersCount = groupedPdfs.Count;
        _viewModel.FoldersScanned = 0;
        foreach (var item in groupedPdfs)
        {
            if (_viewModel.Folders.Any(x => x.AbsolutePath == item.Key))
            {
                _viewModel.FoldersScanned++;
                continue;
            }

            var folderToAdd = new Folder
            {
                AbsolutePath = item.Key,
                Id = Guid.NewGuid(),
                PdfAmount = item.Value.Count
            };

            Application.Current.Dispatcher.Invoke(async () =>
            {
                _viewModel.Folders.Add(folderToAdd);

                foreach (var pdfPath in item.Value)
                {
                    var bookDetail = FilesOperationsHelper.GetBookDetailEntity(pdfPath, folderToAdd.Id);

                    folderToAdd.BookDetails.Add(bookDetail);
                }
            });

            _viewModel.FoldersScanned++;
        }

        await UpdateDatabase();
        _viewModel.DepthSearchEnabled = true;
    }
}