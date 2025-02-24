using System.Collections.ObjectModel;
using System.ComponentModel;
using Infrastructure.Managers;

namespace Presentation.ViewsModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ConfigManager _configManager;

    public ObservableCollection<SettingsChainViewModel> Chains { get; set; } = new();

    public MainViewModel(ConfigManager configManager)
    {
        _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
    }

    public void LoadTemplates()
    {
        Chains.Clear();
        foreach (var chainType in _configManager.GetAllChainTypes())
        {
            var alicePath = _configManager.GetTemplatePath(chainType, "Alice");
            var applePath = _configManager.GetTemplatePath(chainType, "Apple");

            Chains.Add(new SettingsChainViewModel(_configManager)
            {
                Type = chainType,
                AlicePath = string.IsNullOrEmpty(alicePath) ? "Путь не задан" : alicePath,
                ApplePath = string.IsNullOrEmpty(applePath) ? "Путь не задан" : applePath
            });
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}