using FluentValidation;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Application.Validators;

/// <summary>
/// Validator dla CreateTicketRequest.
/// </summary>
public class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
{
    public CreateTicketRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(5).WithMessage("Title must be at least 5 characters")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters")
            .MaximumLength(5000).WithMessage("Description must not exceed 5000 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .Must(c => Enum.TryParse<TicketCategory>(c, true, out _))
            .WithMessage("Invalid ticket category");

        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority is required")
            .Must(p => Enum.TryParse<PriorityLevel>(p, true, out _))
            .WithMessage("Invalid priority level");
    }
}
