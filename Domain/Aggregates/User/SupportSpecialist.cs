using System.Collections.Generic;
using TicketingSystem.Domain.Enums;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.Aggregates.User;

/// <summary>
/// Specjalista supportu - użytkownik odpowiedzialny za rozwiązywanie zgłoszeń.
/// </summary>
public class SupportSpecialist : User
{
    public string? TeamId { get; private set; }
    public string? Specialization { get; private set; }
    public int ActiveTicketLimit { get; private set; }
    public int CurrentActiveCount { get; private set; }

    private SupportSpecialist()
    {
        TeamId = null;
        Specialization = null;
        ActiveTicketLimit = 0;
        CurrentActiveCount = 0;
    }

    private SupportSpecialist(string id, string email, string firstName, string lastName, AccountStatusEnum accountStatus, string? teamId, string? specialization, int activeTicketLimit)
        : base(id, email, firstName, lastName, accountStatus)
    {
        TeamId = teamId;
        Specialization = specialization;
        ActiveTicketLimit = activeTicketLimit;
        CurrentActiveCount = 0;
    }

    public static SupportSpecialist Create(string id, string email, string firstName, string lastName, AccountStatusEnum accountStatus, string? teamId = null, string? specialization = null, int activeTicketLimit = 10)
    {
        ValidateUserData(id, email, firstName, lastName, accountStatus);

        var specialist = new SupportSpecialist(id, email, firstName, lastName, accountStatus, teamId, specialization, activeTicketLimit);
        return specialist;
    }

    public override UserType GetUserType()
    {
        return UserType.SPECIALIST;
    }

    public bool CanAcceptMoreTickets()
    {
        return CurrentActiveCount < ActiveTicketLimit && IsActive();
    }

    public void IncrementActiveTickets()
    {
        if (CurrentActiveCount >= ActiveTicketLimit)
            throw new DomainExceptions.ConflictException("SUPPORT_SPECIALIST_TICKET_LIMIT_ERROR", $"Cannot accept more tickets. Current count: {CurrentActiveCount}, Limit: {ActiveTicketLimit}");

        CurrentActiveCount++;
    }

    public void DecrementActiveTickets()
    {
        if (CurrentActiveCount <= 0)
            return;

        CurrentActiveCount--;
    }

    public override Dictionary<string, object> ToPrimitive()
    {
        var dict = base.ToPrimitive();
        dict["TeamId"] = TeamId ?? string.Empty;
        dict["Specialization"] = Specialization ?? string.Empty;
        dict["ActiveTicketLimit"] = ActiveTicketLimit;
        dict["CurrentActiveCount"] = CurrentActiveCount;
        return dict;
    }
}
