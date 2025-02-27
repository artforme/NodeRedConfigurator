using System.ComponentModel;
using Models.Domain.Entities;

namespace Presentation.ViewsModels;

public class ChainViewModel : INotifyPropertyChanged
{
    private Chain _chain;
    private string _name;

    public Guid Id => _chain.Id;
    public string Type => _chain.Type.Value;

    public string Name
    {
        get => _name;
        private set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public ChainViewModel(Chain chain)
    {
        _chain = chain ?? throw new ArgumentNullException(nameof(chain));
        UpdateFromChain();
    }

    public void Update(Chain updatedChain)
    {
        if (updatedChain == null)
            throw new ArgumentNullException(nameof(updatedChain));

        _chain = updatedChain;
        UpdateFromChain();
        OnPropertyChanged(nameof(Name));
    }
    public void UpdateFromChain()
    {
        Name = _chain.Properties.TryGetValue("Name", out object value) ? value.ToString() : "Unnamed";
        OnPropertyChanged(nameof(Name));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}