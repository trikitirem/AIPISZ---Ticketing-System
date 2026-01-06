using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Validators;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.ValueObjects;

/// <summary>
/// Status konta użytkownika jako Value Object.
/// </summary>
public class AccountStatus : ValueObject
{
    public AccountStatusEnum Status { get; private set; }

    private AccountStatus()
    {
        Status = AccountStatusEnum.INACTIVE;
    }

    private AccountStatus(AccountStatusEnum status)
    {
        Status = status;
    }

    public static AccountStatus Create(AccountStatusEnum status)
    {
        var validator = new AccountStatusValidator();
        var validationResult = validator.Validate(status);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainExceptions.ValidationException("ACCOUNT_STATUS_DATA_VALIDATION_ERROR", errors);
        }

        return new AccountStatus(status);
    }

    public override bool Equals(ValueObject? other)
    {
        if (other is not AccountStatus otherStatus)
            return false;

        return Status == otherStatus.Status;
    }

    public override int GetHashCode()
    {
        return Status.GetHashCode();
    }

    public override string ToString()
    {
        return Status.ToString();
    }
}
