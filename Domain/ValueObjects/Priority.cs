using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Validators;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

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
        var validator = new PriorityValidator();
        var validationResult = validator.Validate(level);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainExceptions.ValidationException("PRIORITY_DATA_VALIDATION_ERROR", errors);
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
