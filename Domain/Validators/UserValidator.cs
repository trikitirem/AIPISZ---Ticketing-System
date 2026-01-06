using FluentValidation;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Validators;

public class UserValidator : AbstractValidator<(string id, string email, string firstName, string lastName, AccountStatusEnum accountStatus)>
{
    public UserValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("User ID cannot be empty")
            .NotNull().WithMessage("User ID cannot be null");

        RuleFor(x => x.email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .NotNull().WithMessage("Email cannot be null")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.firstName)
            .NotEmpty().WithMessage("First name cannot be empty")
            .NotNull().WithMessage("First name cannot be null")
            .MinimumLength(2).WithMessage("First name must be at least 2 characters")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.lastName)
            .NotEmpty().WithMessage("Last name cannot be empty")
            .NotNull().WithMessage("Last name cannot be null")
            .MinimumLength(2).WithMessage("Last name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.accountStatus)
            .IsInEnum().WithMessage("Invalid account status");
    }
}
