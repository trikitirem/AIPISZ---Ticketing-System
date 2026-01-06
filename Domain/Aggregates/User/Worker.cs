using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Domain.Aggregates.User;

/// <summary>
/// Pracownik - użytkownik tworzący zgłoszenia i weryfikujący rozwiązania.
/// </summary>
public class Worker : User
{
    private Worker()
    {
    }

    private Worker(string id, string email, string firstName, string lastName, AccountStatusEnum accountStatus)
        : base(id, email, firstName, lastName, accountStatus)
    {
    }

    public static Worker Create(string id, string email, string firstName, string lastName, AccountStatusEnum accountStatus = AccountStatusEnum.ACTIVE)
    {
        ValidateUserData(id, email, firstName, lastName, accountStatus);

        return new Worker(id, email, firstName, lastName, accountStatus);
    }

    public override UserType GetUserType()
    {
        return UserType.WORKER;
    }

    public bool CanEscalateTicket()
    {
        return IsActive();
    }
}
