using System.Windows;
using Infrastructure.Managers;
using Presentation.ViewsModels;

namespace Presentation.Views;

public partial class ChainSelectionWindow : Window
{
    public ChainSelectionWindow(ChainManager chainManager)
    {
        InitializeComponent();
        var viewModel = new ChainSelectionViewModel(chainManager);
        DataContext = viewModel;
        viewModel.RequestClose += ViewModel_RequestClose;
    }

    private void ViewModel_RequestClose(object sender, EventArgs e)
    {
        Close();
    }
}