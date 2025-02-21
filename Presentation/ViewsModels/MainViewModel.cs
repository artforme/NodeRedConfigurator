using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Presentation.ViewsModels;

public class MainViewModel : INotifyPropertyChanged
{
    public ObservableCollection<ChainViewModel> Chains { get; set; } = new();

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}