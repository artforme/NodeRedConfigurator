using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Presentation.ViewsModels;

namespace Presentation.Views;

public partial class ChainSelectionWindow : Window
{
    public ChainSelectionWindow()
    {
        InitializeComponent();
        DataContext = new ChainSelectionViewModel();
    }
}