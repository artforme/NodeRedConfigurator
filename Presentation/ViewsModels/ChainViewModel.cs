using System.ComponentModel;
using Models.Domain.Entities;

namespace Presentation.ViewsModels;

public class ChainViewModel : INotifyPropertyChanged
{
    private readonly Chain _chain;

    public Guid Id => _chain.Id;
    public string Type => _chain.Type.Value;
    public string Name => _chain.Properties.TryGetValue("Name", out object value) ? value.ToString() : "Unnamed";

    public ChainViewModel(Chain chain)
    {
        _chain = chain ?? throw new ArgumentNullException(nameof(chain));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}