using FluentValidation;
using TicketingSystem.Application.DTOs;

namespace TicketingSystem.Application.Validators;

public class ReviewResolutionRequestValidator : AbstractValidator<ReviewResolutionRequest>
{
    public ReviewResolutionRequestValidator()
    {
        RuleFor(x => x.ReviewComment)
            .NotEmpty().WithMessage("Review comment is required")
            .MinimumLength(5).WithMessage("Comment must be at least 5 characters")
            .MaximumLength(5000).WithMessage("Comment must not exceed 5000 characters");
    }
}
