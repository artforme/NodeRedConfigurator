using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Infrastructure.Managers;
using Infrastructure.Logging;
using Presentation.Helpers;
using Presentation.Services;
using Presentation.Views;
using Presentation.ViewsModels;
using Infrastructure.Generators;
using Infrastructure.JsonProcessing;
using Models.Domain.Entities;
using Models.Domain.ValueObjects;
using Newtonsoft.Json.Linq;

namespace Presentation.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public ChainService ChainService { get; }
    private readonly ConfigManager _configManager;
    private readonly ILogger _logger;

    private string _modelId;
    private string _serialId;

    public ObservableCollection<ChainViewModel> Chains { get; set; } = new();
    public ObservableCollection<SettingsChainViewModel> Templates { get; set; } = new();
    public ICommand RemoveChainCommand { get; }
    public ICommand EditChainCommand { get; }
    public ICommand GenerateCommand { get; }

    public string ModelId
    {
        get => _modelId;
        set
        {
            _modelId = value;
            OnPropertyChanged(nameof(ModelId));
        }
    }

    public string SerialId
    {
        get => _serialId;
        set
        {
            _serialId = value;
            OnPropertyChanged(nameof(SerialId));
        }
    }

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
        GenerateCommand = new RelayCommand(Generate, CanGenerate); // Инициализация команды Generate
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
            var viewModel = new SettingsChainViewModel(_configManager, chainType);
            if (viewModel.IsPlatformIndependent)
            {
                viewModel.SinglePath = _configManager.GetTemplatePath(chainType, "Non platform") ?? "Путь не задан";
            }
            else
            {
                viewModel.AlicePath = _configManager.GetTemplatePath(chainType, "Alice") ?? "Путь не задан";
                viewModel.ApplePath = _configManager.GetTemplatePath(chainType, "Apple") ?? "Путь не задан";
            }
            Templates.Add(viewModel);
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

    private void Generate(object parameter)
    {
        try
        {
            _logger.Info("Starting generation process.");
            
            if (string.IsNullOrEmpty(ModelId) || string.IsNullOrEmpty(SerialId))
            {
                _logger.Warning("Model ID or Serial ID is empty.");
                MessageBox.Show("Please enter Model ID and Serial ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Chains.Any())
            {
                _logger.Warning("No chains available to generate.");
                MessageBox.Show("No chains available to generate.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            var templateManager = new TemplateManager(_configManager.GetTemplatesFolderPath(), _configManager);
            var idGenerator = new IdGenerator();
            var coordinateSetter = new CoordinateSetter();
            var idNodesSetter = new IdNodesSetter(idGenerator);
            var propertiesSetter = new PropertiesSetter();
            
            var updatedGlobalSettings = new GlobalSettings(
                new Info(idGenerator.GenerateSecureIdNodes()), 
                new Info(idGenerator.GenerateSecureIdNodes()), 
                new Info(idGenerator.GenerateSecureIdNodes()),     
                new Info(idGenerator.GenerateSecureIdNodes()),    
                new Info(idGenerator.GenerateSecureIdNodes()),  
                new Info(ModelId),
                new Info(SerialId)
            );

            var jsonManager = new JsonManager(
                templateManager,
                coordinateSetter,
                idNodesSetter,
                propertiesSetter,
                updatedGlobalSettings,
                _logger
            );

            var chains = ChainService.ChainManager.GetAllChains().ToList();

            _logger.Info($"Preparing to generate JSON for {chains.Count} chains: {string.Join(", ", chains.Select(c => $"{c.Type.Value} ({c.Id})"))}");
            var appleJson = jsonManager.GenerateJson(chains, "Apple");
            var aliceJson = jsonManager.GenerateJson(chains, "Alice");
            var connection = jsonManager.GenerateConnectionsJson();

            
            var combinedJson = new JArray();
            foreach (var node in appleJson)
            {
                combinedJson.Add(node);
            }
            foreach (var node in aliceJson)
            {
                combinedJson.Add(node);
            }

            foreach (var node in connection)
            {
                combinedJson.Add(node);
            }
            
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FileName = $"config_{ModelId}_{SerialId}.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, combinedJson.ToString());
                _logger.Info($"JSON file saved to {saveFileDialog.FileName}");
                MessageBox.Show($"File saved successfully to {saveFileDialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _logger.Info("File save operation canceled by user.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error("Error during generation", ex);
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private bool CanGenerate(object parameter)
    {
        return !string.IsNullOrEmpty(ModelId) && !string.IsNullOrEmpty(SerialId) && Chains.Any();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}