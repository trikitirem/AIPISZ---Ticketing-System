using FluentValidation;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Validators;

public class ResolutionValidator : AbstractValidator<(ResolutionType type, string description)>
{
    public ResolutionValidator()
    {
        RuleFor(x => x.type)
            .IsInEnum().WithMessage("Invalid resolution type");

        RuleFor(x => x.description)
            .NotEmpty().WithMessage("Resolution description cannot be empty")
            .NotNull().WithMessage("Resolution description cannot be null");
    }
}
