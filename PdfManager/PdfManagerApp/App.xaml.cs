﻿using System.Configuration;
using System.Data;
using System.Text;
using System.Windows;

namespace PdfManagerApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}
