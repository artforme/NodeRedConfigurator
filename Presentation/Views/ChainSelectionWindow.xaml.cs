using System;
using System.Windows;
using Infrastructure.Logging;
using Models.Domain.Entities;
using Presentation.Services;
using Presentation.ViewModels;

namespace Presentation.Views;

public partial class ChainSelectionWindow : Window
{
    private readonly ChainService _chainService;
    private readonly ILogger _logger;
    private readonly ChainSelectionViewModel _viewModel;

    public ChainSelectionWindow(ChainService chainService, ILogger logger)
    {
        _chainService = chainService ?? throw new ArgumentNullException(nameof(chainService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        InitializeComponent();
        _viewModel = new ChainSelectionViewModel(_chainService, _logger);
        DataContext = _viewModel;
        _viewModel.RequestClose += ViewModel_RequestClose;
        _logger.Info("ChainSelectionWindow initialized with RequestClose subscribed.");
    }

    private void ViewModel_RequestClose(object sender, EventArgs e)
    {
        _logger.Info("ViewModel_RequestClose called.");
        DialogResult = true;
        Close();
    }
}