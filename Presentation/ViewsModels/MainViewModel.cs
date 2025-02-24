using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Infrastructure.Managers;
using Models.Domain.Entities;
using Models.Domain.Entities.RgbStrip;
using Presentation.Helpers;
using Presentation.Views;

namespace Presentation.ViewsModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ConfigManager _configManager;
    private readonly ChainManager _chainManager;

    public ObservableCollection<ChainViewModel> Chains { get; set; } = new(); // Для цепей в MainWindow
    public ObservableCollection<SettingsChainViewModel> Templates { get; set; } = new(); // Для шаблонов в SettingsWindow
    public ICommand RemoveChainCommand { get; }
    public ICommand EditChainCommand { get; }
    public ChainManager ChainManager => _chainManager;

    public MainViewModel(ConfigManager configManager, ChainManager chainManager)
    {
        _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        _chainManager = chainManager ?? throw new ArgumentNullException(nameof(chainManager));
        LoadTemplates(); // Загружаем шаблоны (настройки)
        LoadChains(); // Загружаем цепи

        // Подписываемся на событие добавления цепи
        _chainManager.ChainAdded += ChainManager_ChainAdded;

        RemoveChainCommand = new RelayCommand(RemoveChain, CanRemoveChain);
        EditChainCommand = new RelayCommand(EditChain, CanEditChain);
    }

    private void ChainManager_ChainAdded(object sender, ChainEventArgs e)
    {
        LoadChains(); // Обновляем список цепей при добавлении новой
    }

    public void LoadTemplates()
    {
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
        Chains.Clear();
        foreach (var chain in _chainManager.GetAllChains())
        {
            Chains.Add(new ChainViewModel(chain)); // Используем ChainViewModel для отображения цепей
        }
    }

    private void RemoveChain(object parameter)
    {
        if (parameter is Guid chainId)
        {
            _chainManager.RemoveChain(chainId);
            LoadChains(); // Обновляем UI
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
            var chain = _chainManager.GetChain(chainId);
            if (chain == null)
            {
                Console.WriteLine($"Chain with ID {chainId} not found in ChainManager.");
                return;
            }

            var parameters = CreateParametersFromChain(chain);
            var selectionWindow = new ChainSelectionWindow(_chainManager)
            {
                DataContext = new ChainSelectionViewModel(_chainManager, chain.Type.Value, parameters)
            };
            // Подписываемся на событие закрытия
            var viewModel = (ChainSelectionViewModel)selectionWindow.DataContext;
            viewModel.RequestClose += (sender, e) => selectionWindow.Close(); // Гарантируем закрытие окна
            bool? result = selectionWindow.ShowDialog();
            if (result == true)
            {
                var updatedChain = CreateChainFromParameters(viewModel.SelectedChainType, viewModel.ChainParameters);
                updatedChain.Id = chainId; // Устанавливаем тот же Id, чтобы обновить существующую цепь
                _chainManager.UpdateChain(chainId, updatedChain); // Обновляем существующую цепь
                LoadChains(); // Обновляем UI
            }
        }
    }

    private bool CanEditChain(object parameter)
    {
        return parameter is Guid;
    }

    private ObservableCollection<ChainParameterViewModel> CreateParametersFromChain(Chain chain)
    {
        var parameters = new ObservableCollection<ChainParameterViewModel>();
        foreach (var prop in chain.Properties)
        {
            parameters.Add(new ChainParameterViewModel(prop.Key, GetParameterType(prop.Key), new[] { prop.Value?.ToString() })
            {
                Value = prop.Value // Устанавливаем предыдущее значение
            });
        }
        return parameters;
    }

    private Chain CreateChainFromParameters(string chainType, ObservableCollection<ChainParameterViewModel> parameters)
    {
        Dictionary<string, object> properties = new();
        foreach (var param in parameters)
        {
            properties[param.Name] = param.Value ?? string.Empty;
        }

        return chainType switch
        {
            "RgbStrip" => new RgbStripChain(properties),
            // "Type1" => new Chain(new Info(chainType), properties),
            _ => throw new NotSupportedException($"Chain type {chainType} is not supported.")
        };
    }

    private string GetParameterType(string key)
    {
        return key.Contains("Count") || key.Contains("Index") ? "Number" : "Text";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}