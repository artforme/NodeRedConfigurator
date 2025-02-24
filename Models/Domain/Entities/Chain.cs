using Models.Domain.ValueObjects;

namespace Models.Domain.Entities;

public abstract class Chain
{
    public Guid Id { get; set; }

    public Info Type { get; }
    
    public Dictionary<string, object> Properties { get; }
        
    public Chain(Info type, Dictionary<string, object> properties)
    {
        Id = Guid.NewGuid();
        Type = type;
        Properties = properties;
    }
    
    public abstract (string search, object replace)[] GetProperties();
}