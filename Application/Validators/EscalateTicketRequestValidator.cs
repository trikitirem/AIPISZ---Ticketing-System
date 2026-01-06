using FluentValidation;
using TicketingSystem.Application.DTOs;

namespace TicketingSystem.Application.Validators;

/// <summary>
/// Validator dla EscalateTicketRequest.
/// </summary>
public class EscalateTicketRequestValidator : AbstractValidator<EscalateTicketRequest>
{
    public EscalateTicketRequestValidator()
    {
        RuleFor(x => x.EscalationReason)
            .NotEmpty().WithMessage("Escalation reason is required")
            .MinimumLength(10).WithMessage("Reason must be at least 10 characters")
            .MaximumLength(5000).WithMessage("Reason must not exceed 5000 characters");
    }
}
