using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Infrastructure.Managers;
using Infrastructure.Logging;
using Presentation.Helpers;
using Presentation.Services;
using Presentation.Views;
using Presentation.ViewsModels;

namespace Presentation.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public ChainService ChainService { get; }
    private readonly ConfigManager _configManager;
    private readonly ILogger _logger;

    public ObservableCollection<ChainViewModel> Chains { get; set; } = new();
    public ObservableCollection<SettingsChainViewModel> Templates { get; set; } = new();
    public ICommand RemoveChainCommand { get; }
    public ICommand EditChainCommand { get; }

    public MainViewModel(ConfigManager configManager, ChainService chainService, ILogger logger)
    {
        _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        ChainService = chainService ?? throw new ArgumentNullException(nameof(chainService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        LoadTemplates();
        LoadChains();

        ChainService.ChainManager.ChainAdded += ChainManager_ChainAdded;

        RemoveChainCommand = new RelayCommand(RemoveChain, CanRemoveChain);
        EditChainCommand = new RelayCommand(EditChain, CanEditChain);
    }

    private void ChainManager_ChainAdded(object sender, ChainEventArgs e)
    {
        _logger.Info("Chain added, refreshing chain list.");
        LoadChains();
    }

    public void LoadTemplates()
    {
        _logger.Info("Loading templates.");
        Templates.Clear();
        foreach (var chainType in _configManager.GetAllChainTypes())
        {
            var alicePath = _configManager.GetTemplatePath(chainType, "Alice");
            var applePath = _configManager.GetTemplatePath(chainType, "Apple");

            Templates.Add(new SettingsChainViewModel(_configManager)
            {
                Type = chainType,
                AlicePath = string.IsNullOrEmpty(alicePath) ? "Путь не задан" : alicePath,
                ApplePath = string.IsNullOrEmpty(applePath) ? "Путь не задан" : applePath
            });
        }
    }

    public void LoadChains()
    {
        _logger.Info("Loading chains.");
        Chains.Clear();
        foreach (var chain in ChainService.ChainManager.GetAllChains())
        {
            Chains.Add(new ChainViewModel(chain));
        }
    }

    private void RemoveChain(object parameter)
    {
        if (parameter is Guid chainId)
        {
            _logger.Info($"Removing chain with ID: {chainId}");
            ChainService.ChainManager.RemoveChain(chainId);
            LoadChains();
        }
    }

    private bool CanRemoveChain(object parameter)
    {
        return parameter is Guid;
    }

    private void EditChain(object parameter)
    {
        if (parameter is Guid chainId)
        {
            var chain = ChainService.ChainManager.GetChain(chainId);
            if (chain == null)
            {
                _logger.Warning($"Chain with ID {chainId} not found.");
                Console.WriteLine($"Chain with ID {chainId} not found.");
                return;
            }

            _logger.Info($"Editing chain with ID: {chainId}");
            var selectionWindow = new ChainSelectionWindow(ChainService)
            {
                DataContext = new ChainSelectionViewModel(ChainService, _logger)
            };
            var viewModel = (ChainSelectionViewModel)selectionWindow.DataContext;
            viewModel.RequestClose += (s, e) =>
            {
                if (selectionWindow.DialogResult == true)
                {
                    var updatedChain = ChainService.CreateChain(viewModel.SelectedChainType, viewModel.ChainParameters);
                    ChainService.UpdateChain(chainId, updatedChain);
                    LoadChains();
                }
                selectionWindow.Close();
            };
            selectionWindow.ShowDialog();
        }
    }

    private bool CanEditChain(object parameter)
    {
        return parameter is Guid;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}