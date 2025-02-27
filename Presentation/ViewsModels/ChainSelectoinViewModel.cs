using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Infrastructure.Logging;
using Models.Domain.Entities;
using Presentation.Helpers;
using Presentation.Services;
using Presentation.ViewsModels;

namespace Presentation.ViewModels;

public class ChainSelectionViewModel : INotifyPropertyChanged
{
    public ChainService ChainService { get; }
    private string _selectedChainType;
    private readonly ILogger _logger;
    public bool IsEditing { get; }
    public Guid? ChainId { get; }

    public ObservableCollection<string> ChainTypes { get; }
    public ObservableCollection<ChainParameterViewModel> ChainParameters { get; set; }

    public string SelectedChainType
    {
        get => _selectedChainType;
        set
        {
            _selectedChainType = value;
            OnPropertyChanged(nameof(SelectedChainType));
            LoadParameters();
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public event EventHandler RequestClose;

    public ChainSelectionViewModel(ChainService chainService, ILogger logger)
    {
        ChainService = chainService ?? throw new ArgumentNullException(nameof(chainService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ChainTypes = ChainService.GetChainTypes();
        ChainParameters = new ObservableCollection<ChainParameterViewModel>();
        IsEditing = false;

        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);
    }

    public ChainSelectionViewModel(ChainService chainService, Chain chain, ILogger logger) : this(chainService, logger)
    {
        IsEditing = true;
        ChainId = chain.Id;
        SelectedChainType = chain.Type.Value;
        ChainParameters = MergeParameters(chain); // Загружаем существующие параметры
    }

    private ObservableCollection<ChainParameterViewModel> MergeParameters(Chain chain)
    {
        _logger.Info($"Merging parameters for chain: {chain.Type.Value}");
        var defaultParams = ChainService.GetParametersForChainType(chain.Type.Value);
        var existingParams = ChainService.GetParametersFromChain(chain);

        foreach (var param in existingParams)
        {
            var defaultParam = defaultParams.FirstOrDefault(p => p.Name == param.Name);
            if (defaultParam != null)
            {
                defaultParam.Value = param.Value; // Переносим значения из существующей цепи
            }
        }
        return defaultParams;
    }

    private void LoadParameters()
    {
        _logger.Info($"Loading parameters for chain type: {SelectedChainType}");
        if (!IsEditing) // При редактировании параметры уже загружены
        {
            ChainParameters.Clear();
            if (!string.IsNullOrEmpty(SelectedChainType))
            {
                var parameters = ChainService.GetParametersForChainType(SelectedChainType);
                foreach (var param in parameters)
                {
                    ChainParameters.Add(param);
                }
            }
        }
    }

    private void Save(object parameter)
    {
        if (string.IsNullOrEmpty(SelectedChainType))
        {
            _logger.Warning("Save attempted with no chain type selected.");
            MessageBox.Show("Please select a chain type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            var chain = ChainService.CreateChain(SelectedChainType, ChainParameters);
            if (IsEditing && ChainId.HasValue)
            {
                ChainService.UpdateChain(ChainId.Value, chain);
            }
            else
            {
                ChainService.SaveChain(chain);
            }
            OnRequestClose();
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error("Validation failed during save.", ex);
            MessageBox.Show(ex.Message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            _logger.Error("Unexpected error during save.", ex);
            MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool CanSave(object parameter)
    {
        return !string.IsNullOrEmpty(SelectedChainType);
    }

    private void Cancel(object parameter)
    {
        _logger.Info("Chain selection canceled.");
        OnRequestClose();
    }

    protected virtual void OnRequestClose()
    {
        _logger.Info("OnRequestClose invoked.");
        if (RequestClose == null)
        {
            _logger.Warning("RequestClose event has no subscribers.");
        }
        else
        {
            _logger.Info($"RequestClose has {RequestClose.GetInvocationList().Length} subscribers.");
        }
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}