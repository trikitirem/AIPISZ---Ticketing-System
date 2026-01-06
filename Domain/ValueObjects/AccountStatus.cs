using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Enums;

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
        if (!Enum.IsDefined(typeof(AccountStatusEnum), status))
        {
            throw new ArgumentException($"Invalid account status: {status}", nameof(status));
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
