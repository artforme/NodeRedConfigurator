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

    public string Type { get; set; }

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

    public ICommand SelectAlicePathCommand { get; }
    public ICommand SelectApplePathCommand { get; }

    public SettingsChainViewModel(ConfigManager configManager)
    {
        _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        SelectAlicePathCommand = new RelayCommand(SelectAlicePath, CanSelectPath);
        SelectApplePathCommand = new RelayCommand(SelectApplePath, CanSelectPath);
    }

    private void SelectAlicePath(object parameter)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = _configManager.TemplatesFolderPath.Value // Устанавливаем начальную папку templates
        };
        if (dialog.ShowDialog() == true)
        {
            AlicePath = dialog.FileName;
            _configManager.SetTemplatePath(Type, "Alice", AlicePath); // Сохраняем путь в ConfigManager
        }
    }

    private void SelectApplePath(object parameter)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            InitialDirectory = _configManager.TemplatesFolderPath.Value // Устанавливаем начальную папку templates
        };
        if (dialog.ShowDialog() == true)
        {
            ApplePath = dialog.FileName;
            _configManager.SetTemplatePath(Type, "Apple", ApplePath); // Сохраняем путь в ConfigManager
        }
    }

    private bool CanSelectPath(object parameter)
    {
        return true; // Можно настроить более сложную логику, если нужно
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}