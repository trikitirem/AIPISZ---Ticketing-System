using FluentValidation;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Domain.Validators;

public class TicketNumberValidator : AbstractValidator<string>
{
    public TicketNumberValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Ticket number cannot be empty")
            .NotNull().WithMessage("Ticket number cannot be null");
    }
}
