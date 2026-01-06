using FluentValidation;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Domain.Validators;

public class EscalationValidator : AbstractValidator<(string id, string reason, string escalatedBy, EscalationType escalationType, Priority previousPriority)>
{
    public EscalationValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("Escalation ID cannot be empty")
            .NotNull().WithMessage("Escalation ID cannot be null");

        RuleFor(x => x.reason)
            .NotEmpty().WithMessage("Escalation reason cannot be empty")
            .NotNull().WithMessage("Escalation reason cannot be null");

        RuleFor(x => x.escalatedBy)
            .NotEmpty().WithMessage("EscalatedBy cannot be empty")
            .NotNull().WithMessage("EscalatedBy cannot be null");

        RuleFor(x => x.escalationType)
            .IsInEnum().WithMessage("Invalid escalation type");

        RuleFor(x => x.previousPriority)
            .NotNull().WithMessage("PreviousPriority cannot be null");
    }
}
