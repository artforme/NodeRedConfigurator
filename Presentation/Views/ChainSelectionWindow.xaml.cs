using System;
using System.Windows;
using Infrastructure.Logging;
using Models.Domain.Entities;
using Presentation.Services;
using Presentation.ViewModels;

namespace Presentation.Views;

public partial class ChainSelectionWindow : Window
{
    public ChainSelectionWindow(ChainService chainService)
    {
        InitializeComponent();
        var viewModel = new ChainSelectionViewModel(chainService, new FileLogger());
        DataContext = viewModel;
        viewModel.RequestClose += ViewModel_RequestClose;
    }

    public ChainSelectionWindow(ChainService chainService, Chain chain)
    {
        InitializeComponent();
        var viewModel = new ChainSelectionViewModel(chainService, chain, new FileLogger());
        DataContext = viewModel;
        viewModel.RequestClose += ViewModel_RequestClose;
    }

    private void ViewModel_RequestClose(object sender, EventArgs e)
    {
        DialogResult = true;
        Close();
    }
}