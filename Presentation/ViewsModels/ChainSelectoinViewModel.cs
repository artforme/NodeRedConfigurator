using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
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

    public ChainSelectionViewModel()
    {
        ChainTypes = new ObservableCollection<string> { "Тип 1", "Тип 2", "Тип 3" };
        ChainParameters = new ObservableCollection<ChainParameterViewModel>();

        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    private void LoadParametersForSelectedType()
    {
        ChainParameters.Clear();

        if (SelectedChainType == "Тип 1")
        {
            ChainParameters.Add(new ChainParameterViewModel("Имя", "Text"));
            ChainParameters.Add(new ChainParameterViewModel("Количество", "Number"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
            ChainParameters.Add(new ChainParameterViewModel("Активен", "Boolean"));
        }
        else if (SelectedChainType == "Тип 2")
        {
            ChainParameters.Add(new ChainParameterViewModel("Режим", "Dropdown", new[] { "Авто", "Ручной", "Тест" }));
            ChainParameters.Add(new ChainParameterViewModel("Интенсивность", "Number"));
        }
    }

    private void Save()
    {
        // Логика сохранения
    }

    private void Cancel()
    {
        // Закрытие окна
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}