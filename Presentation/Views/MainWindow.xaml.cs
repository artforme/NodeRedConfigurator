using System.Windows;
using Infrastructure.Managers;

namespace Presentation.Views;

public partial class MainWindow : Window
{
    private readonly ConfigManager _configManager = new ConfigManager("templates.json");

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenSettings_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow(_configManager);
        settingsWindow.ShowDialog();
    }
    
    private void OpenSelectionWindow_Click(object sender, RoutedEventArgs e)
    {
        var selectionWindow = new ChainSelectionWindow();
        selectionWindow.ShowDialog();
    }
}