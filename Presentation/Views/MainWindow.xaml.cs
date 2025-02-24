using System.Windows;
using Infrastructure.Managers;
using Presentation.ViewsModels;

namespace Presentation.Views;

public partial class MainWindow : Window
{
    private readonly ConfigManager _configManager;
    private readonly ChainManager _chainManager;

    public MainWindow()
    {
        _configManager = new ConfigManager();
        _chainManager = new ChainManager();
        InitializeComponent();
        DataContext = new MainViewModel(_configManager, _chainManager);
    }

    private void OpenSettings_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(_configManager);
        settingsWindow.ShowDialog();
    }
    
    private void OpenSelectionWindow_Click(object sender, RoutedEventArgs e)
    {
        var chainManager = ((MainViewModel)DataContext).ChainManager;
        var selectionWindow = new ChainSelectionWindow(chainManager);
        selectionWindow.ShowDialog();
    }
}