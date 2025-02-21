namespace Presentation.ViewsModels;

public class ChainParameterViewModel
{
    public string Name { get; set; }
    public string Type { get; set; }
    public object Value { get; set; }
    public string[] Options { get; set; }

    public ChainParameterViewModel(string name, string type, string[] options = null)
    {
        Name = name;
        Type = type;
        Options = options;
    }
}