using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Validators;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

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
        var validator = new TicketNumberValidator();
        var validationResult = validator.Validate(value);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainExceptions.ValidationException("TICKET_NUMBER_DATA_VALIDATION_ERROR", errors);
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
