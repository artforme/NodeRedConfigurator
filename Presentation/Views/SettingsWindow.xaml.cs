using System.Windows;
using Infrastructure.Logging;
using Presentation.Services;
using Presentation.ViewModels;

namespace Presentation.Views;

public partial class SettingsWindow : Window
{
    private readonly ConfigManager _configManager;
    private readonly ILogger _logger;

    public SettingsWindow(ConfigManager configManager, ChainService chainService, ILogger logger)
    {
        InitializeComponent();
        _configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        DataContext = new MainViewModel(_configManager, chainService, _logger);
        LoadTemplates();
    }

    private void LoadTemplates()
    {
        _logger.Info("Loading templates in Settings window.");
        ((MainViewModel)DataContext).LoadTemplates();
    }
}