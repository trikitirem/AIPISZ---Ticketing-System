using FluentValidation;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Validators;

public class TeamValidator : AbstractValidator<(string id, string name, TicketCategory specialization, int maxTickets)>
{
    public TeamValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("Team ID cannot be empty")
            .NotNull().WithMessage("Team ID cannot be null");

        RuleFor(x => x.name)
            .NotEmpty().WithMessage("Team name cannot be empty")
            .NotNull().WithMessage("Team name cannot be null")
            .MinimumLength(2).WithMessage("Team name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Team name must not exceed 100 characters");

        RuleFor(x => x.specialization)
            .IsInEnum().WithMessage("Invalid ticket category for specialization");

        RuleFor(x => x.maxTickets)
            .GreaterThan(0).WithMessage("Max tickets must be greater than 0")
            .LessThanOrEqualTo(1000).WithMessage("Max tickets must not exceed 1000");
    }
}
