using FluentValidation;

namespace TicketingSystem.Domain.Validators;

public class SatisfactionValidator : AbstractValidator<(string id, int rating)>
{
    public SatisfactionValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("Satisfaction ID cannot be empty")
            .NotNull().WithMessage("Satisfaction ID cannot be null");

        RuleFor(x => x.rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
    }
}
