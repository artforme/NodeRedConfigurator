namespace Models.Domain.Entities.RgbStrip;

public class RgbStripChain : Chain
{
    public RgbStripChain(Dictionary<string, object> properties) : base(properties) { }

    public override (string search, string replace)[] GetProperties()
    {
        throw new NotImplementedException();
    }
}
