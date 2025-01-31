namespace Models.Domain.ValueObjects;

public record Info
{
    public string Value { get; }

    public Info(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The Info can't be blank.", nameof(value));
        }

        Value = value;
    }
}