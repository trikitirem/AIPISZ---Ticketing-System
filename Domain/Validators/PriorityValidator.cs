using FluentValidation;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Domain.Validators;

public class PriorityValidator : AbstractValidator<PriorityLevel>
{
    public PriorityValidator()
    {
        RuleFor(x => x)
            .IsInEnum().WithMessage("Invalid priority level");
    }
}
