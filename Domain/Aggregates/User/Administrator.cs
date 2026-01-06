using TicketingSystem.Domain.Enums;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.Aggregates.User;

/// <summary>
/// Administrator systemu - użytkownik z pełnymi uprawnieniami zarządzania.
/// </summary>
public class Administrator : User
{
    private Administrator()
    {
    }

    private Administrator(string id, string email, string firstName, string lastName, AccountStatusEnum accountStatus)
        : base(id, email, firstName, lastName, accountStatus)
    {
    }

    public static Administrator Create(string id, string email, string firstName, string lastName, AccountStatusEnum accountStatus = AccountStatusEnum.ACTIVE)
    {
        ValidateUserData(id, email, firstName, lastName, accountStatus);

        return new Administrator(id, email, firstName, lastName, accountStatus);
    }

    public override UserType GetUserType()
    {
        return UserType.ADMINISTRATOR;
    }

    public void EscalateTicket(string ticketId)
    {
        if (string.IsNullOrWhiteSpace(ticketId))
            throw new DomainExceptions.ValidationException("ADMIN_ESCALATION_DATA_VALIDATION_ERROR", "Ticket ID cannot be null or empty");
    }

    public void ManagePolicies()
    {
        if (!IsActive())
            throw new DomainExceptions.ForbiddenException("ADMIN_ACCESS_DENIED", "Administrator account must be active to manage policies");
    }
}
