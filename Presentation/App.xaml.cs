using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Presentation;

public partial class App : Application
{
    private void OnNumberValidation(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("^[0-9]+$");
        e.Handled = !regex.IsMatch(e.Text);
    }
}