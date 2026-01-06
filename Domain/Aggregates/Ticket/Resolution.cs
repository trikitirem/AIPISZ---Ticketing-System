using System.Collections.Generic;
using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Validators;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Domain.Aggregates.Ticket;

/// <summary>
/// Rozwiązanie zgłoszenia jako Value Object.
/// </summary>
public class Resolution : ValueObject
{
    public ResolutionType Type { get; private set; }
    public string Description { get; private set; }
    public List<string> Tags { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Resolution()
    {
        Description = string.Empty;
        Tags = new List<string>();
    }

    private Resolution(ResolutionType type, string description, List<string>? tags)
    {
        Type = type;
        Description = description;
        Tags = tags ?? new List<string>();
        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Resolution> Create(ResolutionType type, string description, List<string>? tags = null)
    {
        var validator = new ResolutionValidator();
        var validationResult = validator.Validate((type, description));

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<Resolution>.CreateFailure($"RESOLUTION_DATA_VALIDATION_ERROR: {errors}");
        }

        var resolution = new Resolution(type, description, tags);
        return Result<Resolution>.CreateSuccess(resolution);
    }

    public override bool Equals(ValueObject? other)
    {
        if (other is not Resolution otherResolution)
            return false;

        return Type == otherResolution.Type &&
               Description == otherResolution.Description &&
               Tags.SequenceEqual(otherResolution.Tags);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Description, Tags);
    }
}
