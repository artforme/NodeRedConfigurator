using System.Windows.Input;
using Microsoft.Win32;
using Presentation.Helpers;

public class ChainViewModel
{
    public string Type { get; set; }
    public string AlicePath { get; set; }
    public string ApplePath { get; set; }

    public ICommand SelectAlicePathCommand { get; }
    public ICommand SelectApplePathCommand { get; }

    public ChainViewModel()
    {
        SelectAlicePathCommand = new RelayCommand(SelectAlicePath);
        SelectApplePathCommand = new RelayCommand(SelectApplePath);
    }

    private void SelectAlicePath()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
        };
        if (dialog.ShowDialog() == true)
        {
            AlicePath = dialog.FileName;
        }
    }

    private void SelectApplePath()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
        };
        if (dialog.ShowDialog() == true)
        {
            ApplePath = dialog.FileName;
        }
    }
}