using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Validators;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Domain.Aggregates.Ticket;

/// <summary>
/// Eskalacja zgłoszenia.
/// </summary>
public class Escalation : Entity
{
    public string Reason { get; private set; }
    public Priority PreviousPriority { get; private set; }
    public Priority? NewPriority { get; private set; }
    public DateTime EscalatedAt { get; private set; }
    public string EscalatedBy { get; private set; }
    public EscalationType EscalationType { get; private set; }

    private Escalation()
    {
        Reason = string.Empty;
        EscalatedBy = string.Empty;
        PreviousPriority = Priority.Create(PriorityLevel.NISKI);
    }

    private Escalation(string id, string reason, string escalatedBy, EscalationType escalationType, Priority previousPriority, Priority? newPriority)
    {
        Id = id;
        Reason = reason;
        EscalatedBy = escalatedBy;
        EscalationType = escalationType;
        PreviousPriority = previousPriority;
        NewPriority = newPriority;
        EscalatedAt = DateTime.UtcNow;
    }

    public static Result<Escalation> Create(string id, string reason, string escalatedBy, EscalationType escalationType, Priority previousPriority, Priority? newPriority = null)
    {
        var validator = new EscalationValidator();
        var validationResult = validator.Validate((id, reason, escalatedBy, escalationType, previousPriority));

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return Result<Escalation>.CreateFailure($"ESCALATION_DATA_VALIDATION_ERROR: {errors}");
        }

        var escalation = new Escalation(id, reason, escalatedBy, escalationType, previousPriority, newPriority);
        return Result<Escalation>.CreateSuccess(escalation);
    }

    public override Dictionary<string, object> ToPrimitive()
    {
        var dict = new Dictionary<string, object>
        {
            { "Id", Id },
            { "Reason", Reason },
            { "EscalatedBy", EscalatedBy },
            { "EscalationType", EscalationType.ToString() },
            { "PreviousPriority", PreviousPriority.Level.ToString() },
            { "EscalatedAt", EscalatedAt }
        };

        if (NewPriority is not null)
        {
            dict["NewPriority"] = NewPriority.Level.ToString();
        }

        return dict;
    }
}
