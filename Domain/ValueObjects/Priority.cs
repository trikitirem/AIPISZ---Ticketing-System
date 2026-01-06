using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.ValueObjects;

/// <summary>
/// Priorytet zgłoszenia jako Value Object.
/// </summary>
public class Priority : ValueObject
{
    public PriorityLevel Level { get; private set; }

    private Priority()
    {
        Level = PriorityLevel.NISKI;
    }

    private Priority(PriorityLevel level)
    {
        Level = level;
    }

    public static Priority Create(PriorityLevel level)
    {
        if (!Enum.IsDefined(typeof(PriorityLevel), level))
        {
            throw new ArgumentException($"Invalid priority level: {level}", nameof(level));
        }

        return new Priority(level);
    }

    public override bool Equals(ValueObject? other)
    {
        if (other is not Priority otherPriority)
            return false;

        return Level == otherPriority.Level;
    }

    public override int GetHashCode()
    {
        return Level.GetHashCode();
    }

    public override string ToString()
    {
        return Level.ToString();
    }
}
