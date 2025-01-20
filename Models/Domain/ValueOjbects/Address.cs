namespace Models.Domain.ValueOjbects;

public record Address
{
    public int Value { get; }

    public Address(int value)
    {
        if (value < 1 || value > 255)
        {
            throw new ArgumentException("The address must be in the range 1-255.", nameof(value));
        }

        Value = value;
    }
}