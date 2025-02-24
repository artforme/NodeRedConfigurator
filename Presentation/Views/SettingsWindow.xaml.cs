using System.Windows;
using Infrastructure.Managers;
using Presentation.ViewsModels;

namespace Presentation.Views;

public partial class SettingsWindow : Window
{
    private readonly ConfigManager _configManager;
    public MainViewModel ViewModel { get; }

    public SettingsWindow(ConfigManager configManager)
    {
        InitializeComponent();

        _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        ViewModel = new MainViewModel(_configManager, new ChainManager());
        DataContext = ViewModel;

        LoadTemplates();
    }

    private void LoadTemplates()
    {
        ViewModel.LoadTemplates();
    }
}