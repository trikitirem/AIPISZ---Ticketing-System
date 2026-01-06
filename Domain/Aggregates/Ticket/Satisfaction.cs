using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Validators;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.Aggregates.Ticket;

/// <summary>
/// Satysfakcja workera z rozwiązania zgłoszenia.
/// </summary>
public class Satisfaction : Entity<string>
{
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public DateTime FilledAt { get; private set; }
    public bool IsProblemResolved { get; private set; }

    private Satisfaction()
    {
    }

    private Satisfaction(string id, int rating, string? comment, bool isProblemResolved)
    {
        Id = id;
        Rating = rating;
        Comment = comment;
        IsProblemResolved = isProblemResolved;
        FilledAt = DateTime.UtcNow;
    }

    public static Satisfaction Create(string id, int rating, string? comment, bool isProblemResolved)
    {
        var validator = new SatisfactionValidator();
        var validationResult = validator.Validate((id, rating));

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainExceptions.ValidationException("SATISFACTION_DATA_VALIDATION_ERROR", errors);
        }

        return new Satisfaction(id, rating, comment, isProblemResolved);
    }

    public override Dictionary<string, object> ToPrimitive()
    {
        var dict = new Dictionary<string, object>
        {
            { "Id", Id },
            { "Rating", Rating },
            { "IsProblemResolved", IsProblemResolved },
            { "FilledAt", FilledAt }
        };

        if (Comment is not null)
            dict["Comment"] = Comment;

        return dict;
    }
}
