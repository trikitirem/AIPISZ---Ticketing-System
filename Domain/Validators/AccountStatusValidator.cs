using FluentValidation;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Validators;

public class AccountStatusValidator : AbstractValidator<AccountStatusEnum>
{
    public AccountStatusValidator()
    {
        RuleFor(x => x)
            .IsInEnum().WithMessage("Invalid account status");
    }
}
