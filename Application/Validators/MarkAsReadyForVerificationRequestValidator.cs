using FluentValidation;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Application.Validators;

public class MarkAsReadyForVerificationRequestValidator : AbstractValidator<MarkAsReadyForVerificationRequest>
{
    public MarkAsReadyForVerificationRequestValidator()
    {
        RuleFor(x => x.ResolutionDescription)
            .NotEmpty().WithMessage("Resolution description is required")
            .MinimumLength(10).WithMessage("Description must be at least 10 characters");

        RuleFor(x => x.ResolutionType)
            .NotEmpty().WithMessage("Resolution type is required")
            .Must(rt => Enum.TryParse<ResolutionType>(rt, true, out _))
            .WithMessage("Invalid resolution type");
    }
}
