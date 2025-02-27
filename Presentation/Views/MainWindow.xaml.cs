using System.Windows;
using Infrastructure.Managers;
using Infrastructure.Logging;
using Presentation.Services;
using Presentation.ViewModels;

namespace Presentation.Views;

public partial class MainWindow : Window
{
    private readonly ConfigManager _configManager;
    private readonly ChainManager _chainManager;
    private readonly ChainService _chainService;
    private readonly ILogger _logger;

    public MainWindow()
    {
        _logger = new FileLogger();
        _configManager = new ConfigManager();
        _chainManager = new ChainManager();
        _chainService = new ChainService(_chainManager, _logger);
        InitializeComponent();
        DataContext = new MainViewModel(_configManager, _chainService, _logger);
    }

    private void OpenSettings_Click(object sender, RoutedEventArgs e)
    {
        _logger.Info("Opening Settings window.");
        var settingsWindow = new SettingsWindow(_configManager, _chainService, _logger);
        settingsWindow.ShowDialog();
    }
    
    private void OpenSelectionWindow_Click(object sender, RoutedEventArgs e)
    {
        _logger.Info("Opening Chain Selection window.");
        var chainService = ((MainViewModel)DataContext).ChainService;
        var selectionWindow = new ChainSelectionWindow(chainService, _logger);
        selectionWindow.ShowDialog();
    }
}