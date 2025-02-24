using System.Windows;


namespace Presentation.Views;

public partial class MainWindow : Window
{
    private readonly ConfigManager _configManager;

    public MainWindow()
    {
        _configManager = new ConfigManager();
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