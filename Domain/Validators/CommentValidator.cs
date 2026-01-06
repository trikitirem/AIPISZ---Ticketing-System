using FluentValidation;

namespace TicketingSystem.Domain.Validators;

public class CommentValidator : AbstractValidator<(string id, string authorId, string content)>
{
    public CommentValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("Comment ID cannot be empty")
            .NotNull().WithMessage("Comment ID cannot be null");

        RuleFor(x => x.authorId)
            .NotEmpty().WithMessage("Author ID cannot be empty")
            .NotNull().WithMessage("Author ID cannot be null");

        RuleFor(x => x.content)
            .NotEmpty().WithMessage("Content cannot be empty")
            .NotNull().WithMessage("Content cannot be null");
    }
}
