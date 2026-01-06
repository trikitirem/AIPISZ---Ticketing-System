using TicketingSystem.Domain.Base;

namespace TicketingSystem.Domain.ValueObjects;

/// <summary>
/// Numer zgłoszenia jako Value Object.
/// </summary>
public class TicketNumber : ValueObject
{
    public string Value { get; private set; }

    private TicketNumber()
    {
        Value = string.Empty;
    }

    private TicketNumber(string value)
    {
        Value = value;
    }

    public static TicketNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Ticket number cannot be null or empty.", nameof(value));
        }

        return new TicketNumber(value);
    }

    public override bool Equals(ValueObject? other)
    {
        if (other is not TicketNumber otherNumber)
            return false;

        return Value == otherNumber.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }
}
