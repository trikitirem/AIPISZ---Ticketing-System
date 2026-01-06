using FluentValidation;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Validators;

public class TicketValidator : AbstractValidator<(string id, string title, string description, TicketCategory category, PriorityLevel priorityLevel, string createdById)>
{
    public TicketValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("Ticket ID cannot be empty")
            .NotNull().WithMessage("Ticket ID cannot be null");

        RuleFor(x => x.title)
            .NotEmpty().WithMessage("Title cannot be empty")
            .NotNull().WithMessage("Title cannot be null");

        RuleFor(x => x.description)
            .NotEmpty().WithMessage("Description cannot be empty")
            .NotNull().WithMessage("Description cannot be null");

        RuleFor(x => x.category)
            .IsInEnum().WithMessage("Invalid ticket category");

        RuleFor(x => x.priorityLevel)
            .IsInEnum().WithMessage("Invalid priority level");

        RuleFor(x => x.createdById)
            .NotEmpty().WithMessage("CreatedById cannot be empty")
            .NotNull().WithMessage("CreatedById cannot be null");
    }
}
