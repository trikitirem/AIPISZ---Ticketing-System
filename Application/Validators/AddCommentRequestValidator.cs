using FluentValidation;
using TicketingSystem.Application.DTOs;

namespace TicketingSystem.Application.Validators;

public class AddCommentRequestValidator : AbstractValidator<AddCommentRequest>
{
    public AddCommentRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MinimumLength(1).WithMessage("Content must be at least 1 character")
            .MaximumLength(5000).WithMessage("Content must not exceed 5000 characters");
    }
}
