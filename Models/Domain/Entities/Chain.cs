namespace Models.Domain.Entities;

public abstract class Chain
{
    protected readonly Dictionary<string, object> Properties;
        
    public Chain(Dictionary<string, object> properties)
    {
        Properties = properties;
    }
    
    public abstract (string search, string replace)[] GetProperties();
}