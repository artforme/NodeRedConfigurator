namespace Models.Domain.Entities.RgbStrip;

public class RgbStripChain : Chain
{
    public RgbStripChain(Dictionary<string, object> properties) : base(properties) { }

    public override (string search, object replace)[] GetProperties()
    {
        var keysToExtract = new List<string>
        {
            "%%name%%",
            "%%device%%",
            "%%dev_address%%",
            "%%dev_index%%",
            "%%room%%"
        };
        
        var propertiesList = new List<(string search, object replace)>();
        
        foreach (var key in keysToExtract)
        {
            if (Properties.TryGetValue(key, out var value))
            {
                propertiesList.Add((key, value));
            }
            else
            {
                throw new KeyNotFoundException($"Property '{key}' not found in the properties dictionary.");
            }
        }
        
        return propertiesList.ToArray();
    }
}
