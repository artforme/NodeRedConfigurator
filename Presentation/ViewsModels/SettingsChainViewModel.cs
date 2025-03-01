using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Win32;
using Presentation.Helpers;

namespace Presentation.ViewsModels;

public class SettingsChainViewModel : INotifyPropertyChanged
{
    private readonly ConfigManager _configManager;
    private string _alicePath;
    private string _applePath;
    private string _singlePath; // Для цепочек без платформ

    public string Type { get; set; }
    public bool IsPlatformIndependent { get; } // Флаг для цепочек без платформ

    public string AlicePath
    {
        get => _alicePath;
        set
        {
            _alicePath = value;
            OnPropertyChanged(nameof(AlicePath));
        }
    }

    public string ApplePath
    {
        get => _applePath;
        set
        {
            _applePath = value;
            OnPropertyChanged(nameof(ApplePath));
        }
    }

    public string SinglePath
    {
        get => _singlePath;
        set
        {
            _singlePath = value;
            OnPropertyChanged(nameof(SinglePath));
        }
    }

    public ICommand SelectAlicePathCommand { get; }
    public ICommand SelectApplePathCommand { get; }
    public ICommand SelectSinglePathCommand { get; }

    public SettingsChainViewModel(ConfigManager configManager, string type)
    {
        _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        Type = type;
        IsPlatformIndependent = Type == "Connection"; // Определяем, независим ли тип от платформ

        SelectAlicePathCommand = new RelayCommand(SelectAlicePath, CanSelectPath);
        SelectApplePathCommand = new RelayCommand(SelectApplePath, CanSelectPath);
        SelectSinglePathCommand = new RelayCommand(SelectSinglePath, CanSelectPath);

        // Загружаем существующие пути
        if (IsPlatformIndependent)
        {
            SinglePath = _configManager.GetTemplatePath(Type, "Non platform");
        }
        else
        {
            AlicePath = _configManager.GetTemplatePath(Type, "Alice");
            ApplePath = _configManager.GetTemplatePath(Type, "Apple");
        }
    }

    private void SelectAlicePath(object parameter)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = _configManager.TemplatesFolderPath.Value
        };
        if (dialog.ShowDialog() == true)
        {
            AlicePath = dialog.FileName;
            _configManager.SetTemplatePath(Type, "Alice", AlicePath);
        }
    }

    private void SelectApplePath(object parameter)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = _configManager.TemplatesFolderPath.Value
        };
        if (dialog.ShowDialog() == true)
        {
            ApplePath = dialog.FileName;
            _configManager.SetTemplatePath(Type, "Apple", ApplePath);
        }
    }

    private void SelectSinglePath(object parameter)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = _configManager.TemplatesFolderPath.Value
        };
        if (dialog.ShowDialog() == true)
        {
            SinglePath = dialog.FileName;
            _configManager.SetTemplatePath(Type, "Non platform", SinglePath);
        }
    }

    private bool CanSelectPath(object parameter) => true;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}