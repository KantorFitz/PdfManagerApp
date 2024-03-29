﻿using System.IO;
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

    private async void AddToFolderList_OnClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel.ChosenFolderPath.IsNullOrEmpty())
            return;
        
        if (!Directory.Exists(_viewModel.ChosenFolderPath))
            return;
        
        if (_viewModel.Folders.Any(x => x.AbsolutePath == _viewModel.ChosenFolderPath))
            return;
        
        await UpdatePdfList(_viewModel.ChosenFolderPath);
    }

    private async Task UpdatePdfList(string path)
    {
        _viewModel.CanPerformFilesSearch = false;
        _viewModel.TotalFoldersCount = 1;
        _viewModel.FoldersScanned = 0;
        
        var obtainedPdfs = FilesOperationsHelper.GetFolderPdfs(path).ToList();

        await Task.Run(async () =>
        {
            var folderToAdd = new Folder
            {
                AbsolutePath = _viewModel.ChosenFolderPath,
                Id = Guid.NewGuid(),
                PdfAmount = obtainedPdfs.Count
            };

            Application.Current.Dispatcher.InvokeAsync(() => { _viewModel.Folders.Add(folderToAdd); });

            foreach (var pdfPath in obtainedPdfs)
            {
                var bookDetail = await FilesOperationsHelper.GetBookDetailedEntityAsync(pdfPath, folderToAdd.Id);

                Application.Current.Dispatcher.InvokeAsync(() => { folderToAdd.BookDetails.Add(bookDetail); });
            }
            
            await UpdateDatabase();
            _viewModel.CanPerformFilesSearch = true;
        });
    }

    private async void BtnAddDepthFiles_OnClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel.ChosenFolderPath.IsNullOrEmpty())
            return;
        
        if (!Directory.Exists(_viewModel.ChosenFolderPath))
            return;

        _viewModel.CanPerformFilesSearch = false;
        await FillList(FilesOperationsHelper.GetFolderPdfsDepth(_viewModel.ChosenFolderPath));
    }

    private async Task FillList(IDictionary<string, List<string>> groupedPdfs)
    {
        _viewModel.TotalFoldersCount = groupedPdfs.Count;
        _viewModel.FoldersScanned = 0;

        await Task.Run(async () =>
        {
            foreach (var item in groupedPdfs.OrderBy(x => x.Value.Count))
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

                Application.Current.Dispatcher.InvokeAsync(() => { _viewModel.Folders.Add(folderToAdd); });

                foreach (var pdfPath in item.Value)
                {
                    var bookDetail = await FilesOperationsHelper.GetBookDetailedEntityAsync(pdfPath, folderToAdd.Id);

                    Application.Current.Dispatcher.InvokeAsync(() => { folderToAdd.BookDetails.Add(bookDetail); });
                }

                _viewModel.FoldersScanned++;
            }

            await UpdateDatabase();
            _viewModel.CanPerformFilesSearch = true;
        });
    }
}
