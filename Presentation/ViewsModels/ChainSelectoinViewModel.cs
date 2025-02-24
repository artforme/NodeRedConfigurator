using Models.Domain.Entities;
using Models.Domain.Entities.RgbStrip;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Infrastructure.Managers;
using Models.Domain.ValueObjects;
using Presentation.Helpers;

namespace Presentation.ViewsModels;

public class ChainSelectionViewModel : INotifyPropertyChanged
{
    private string _selectedChainType;
    public ObservableCollection<string> ChainTypes { get; set; }
    public ObservableCollection<ChainParameterViewModel> ChainParameters { get; set; }

    public string SelectedChainType
    {
        get => _selectedChainType;
        set
        {
            _selectedChainType = value;
            OnPropertyChanged(nameof(SelectedChainType));
            LoadParametersForSelectedType();
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private readonly ChainManager _chainManager;

    // Событие для уведомления View о необходимости закрытия
    public event EventHandler RequestClose;

    public ChainSelectionViewModel(ChainManager chainManager)
    {
        _chainManager = chainManager ?? throw new ArgumentNullException(nameof(chainManager));
        ChainTypes = new ObservableCollection<string> { "RgbStrip", "Type1", "Type2", "Type3" }; // Загрузите из ConfigManager, если нужно
        ChainParameters = new ObservableCollection<ChainParameterViewModel>();

        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);
    }

    // Конструктор для редактирования существующей цепи
    public ChainSelectionViewModel(ChainManager chainManager, string chainType, ObservableCollection<ChainParameterViewModel> parameters) : this(chainManager)
    {
        SelectedChainType = chainType;
        if (parameters != null)
        {
            ChainParameters.Clear();
            foreach (var param in parameters)
            {
                ChainParameters.Add(new ChainParameterViewModel(param.Name, param.Type, param.Options)
                {
                    Value = param.Value // Устанавливаем предыдущее значение
                });
            }
        }
    }

    private void LoadParametersForSelectedType()
    {
        ChainParameters.Clear();

        switch (SelectedChainType)
        {
            case "RgbStrip":
                ChainParameters.Add(new ChainParameterViewModel("Name", "Text"));
                ChainParameters.Add(new ChainParameterViewModel("Device", "Text"));
                ChainParameters.Add(new ChainParameterViewModel("DevAddress", "Text"));
                ChainParameters.Add(new ChainParameterViewModel("DevIndex", "Number"));
                ChainParameters.Add(new ChainParameterViewModel("Room", "Text"));
                break;
            case "Type1":
                ChainParameters.Add(new ChainParameterViewModel("Name", "Text"));
                ChainParameters.Add(new ChainParameterViewModel("Count", "Number"));
                ChainParameters.Add(new ChainParameterViewModel("Active", "Boolean"));
                break;
        }
    }

    private void Save(object parameter)
    {
        if (string.IsNullOrEmpty(SelectedChainType))
        {
            return;
        }

        Dictionary<string, object> properties = new();
        foreach (var param in ChainParameters)
        {
            properties[param.Name] = param.Value ?? string.Empty;
        }

        Chain chain;
        switch (SelectedChainType)
        {
            case "RgbStrip":
                chain = new RgbStripChain(properties);
                break;
            default:
                throw new NotSupportedException($"Chain type {SelectedChainType} is not supported.");
        }

        Guid chainId = _chainManager.AddChain(chain);
        Console.WriteLine($"Created/Updated chain with ID: {chainId} (Chain ID: {chain.Id})"); // Логируем оба Id
        OnRequestClose(); // Уведомляем View о закрытии после сохранения
    }

    private bool CanSave(object parameter)
    {
        return !string.IsNullOrEmpty(SelectedChainType);
    }

    private void Cancel(object parameter)
    {
        OnRequestClose(); // Уведомляем View о закрытии при отмене
    }

    protected virtual void OnRequestClose()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}