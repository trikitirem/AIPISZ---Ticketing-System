using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Validators;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.Aggregates.Ticket;

/// <summary>
/// Historia zmian zgłoszenia.
/// </summary>
public class HistoryChange : Entity<string>
{
    public DateTime ChangedAt { get; private set; }
    public string ChangeType { get; private set; }
    public string? PreviousValue { get; private set; }
    public string? NewValue { get; private set; }
    public string PerformedBy { get; private set; }
    public string? Description { get; private set; }

    private HistoryChange()
    {
        ChangeType = string.Empty;
        PerformedBy = string.Empty;
    }

    private HistoryChange(string id, string changeType, string? previousValue, string? newValue, string performedBy, string? description)
    {
        Id = id;
        ChangeType = changeType;
        PreviousValue = previousValue;
        NewValue = newValue;
        PerformedBy = performedBy;
        Description = description;
        ChangedAt = DateTime.UtcNow;
    }

    public static HistoryChange Create(string id, string changeType, string? previousValue, string? newValue, string performedBy, string? description = null)
    {
        var validator = new HistoryChangeValidator();
        var validationResult = validator.Validate((id, changeType, performedBy));

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainExceptions.ValidationException("HISTORY_CHANGE_DATA_VALIDATION_ERROR", errors);
        }

        return new HistoryChange(id, changeType, previousValue, newValue, performedBy, description);
    }

    public override Dictionary<string, object> ToPrimitive()
    {
        var dict = new Dictionary<string, object>
        {
            { "Id", Id },
            { "ChangeType", ChangeType },
            { "PerformedBy", PerformedBy },
            { "ChangedAt", ChangedAt }
        };

        if (PreviousValue is not null)
            dict["PreviousValue"] = PreviousValue;

        if (NewValue is not null)
            dict["NewValue"] = NewValue;

        if (Description is not null)
            dict["Description"] = Description;

        return dict;
    }
}
