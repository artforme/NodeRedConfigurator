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
        _logger.Info("Loading chains...");
        var updatedChains = ChainService.ChainManager.GetAllChains();
        foreach (var chain in updatedChains)
        {
            _logger.Info($"Chain from manager: {chain.Id}, Name: {chain.Properties["Name"]}");
            var existing = Chains.FirstOrDefault(c => c.Id == chain.Id);
            if (existing != null)
            {
                existing.Update(chain);
                _logger.Info($"Updated ChainViewModel: {existing.Id}, Name: {existing.Name}");
            }
            else
            {
                var newChainVM = new ChainViewModel(chain);
                Chains.Add(newChainVM);
                _logger.Info($"Added new ChainViewModel: {newChainVM.Id}, Name: {newChainVM.Name}");
            }
        }
        var toRemove = Chains.Where(c => !updatedChains.Any(uc => uc.Id == c.Id)).ToList();
        foreach (var remove in toRemove)
        {
            _logger.Info($"Removing ChainViewModel: {remove.Id}, Name: {remove.Name}");
            Chains.Remove(remove);
        }
        _logger.Info($"Chains count after update: {Chains.Count}");
        OnPropertyChanged(nameof(Chains));
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
                return;
            }
            var selectionWindow = new ChainSelectionWindow(ChainService, _logger);
            var viewModel = new ChainSelectionViewModel(ChainService, chain, _logger);
            selectionWindow.DataContext = viewModel;
            viewModel.RequestClose += (s, e) =>
            {
                _logger.Info($"RequestClose triggered. DialogResult: {selectionWindow.DialogResult}");
                if (selectionWindow.DialogResult == true)
                {
                    LoadChains();
                    _logger.Info("LoadChains called after edit.");
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