using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Domain.Policies;

/// <summary>
/// Policy walidująca przejścia między statusami zgłoszeń.
/// </summary>
public class TicketStatusPolicy : Policy
{
    /// <summary>
    /// Sprawdza czy można przejść z obecnego statusu do docelowego.
    /// </summary>
    public Result<bool> CanTransitionTo(TicketStatus current, TicketStatus target, UserType performedBy)
    {
        if (current == target)
        {
            return Success();
        }

        return current switch
        {
            TicketStatus.NOWE => CanTransitionFromNowe(target, performedBy),
            TicketStatus.PRZYPISANE => CanTransitionFromPrzypisane(target, performedBy),
            TicketStatus.W_TOKU => CanTransitionFromWToku(target, performedBy),
            TicketStatus.OCZEKUJE_NA_ODPOWIEDZ => CanTransitionFromOczekujeNaOdpowiedz(target, performedBy),
            TicketStatus.GOTOWE_DO_WERYFIKACJI => CanTransitionFromGotoweDoWeryfikacji(target, performedBy),
            TicketStatus.ESKALOWANE => CanTransitionFromEskalowane(target, performedBy),
            TicketStatus.ZAMKNIETE => Failure("Cannot transition from ZAMKNIETE status"),
            _ => Failure($"Unknown current status: {current}")
        };
    }

    private Result<bool> CanTransitionFromNowe(TicketStatus target, UserType performedBy)
    {
        return target switch
        {
            TicketStatus.PRZYPISANE => Success(),
            TicketStatus.ESKALOWANE when performedBy == UserType.ADMINISTRATOR => Success(),
            _ => Failure($"Cannot transition from NOWE to {target}")
        };
    }

    private Result<bool> CanTransitionFromPrzypisane(TicketStatus target, UserType performedBy)
    {
        return target switch
        {
            TicketStatus.W_TOKU when performedBy == UserType.SPECIALIST => Success(),
            TicketStatus.ESKALOWANE when performedBy == UserType.ADMINISTRATOR => Success(),
            _ => Failure($"Cannot transition from PRZYPISANE to {target}")
        };
    }

    private Result<bool> CanTransitionFromWToku(TicketStatus target, UserType performedBy)
    {
        return target switch
        {
            TicketStatus.GOTOWE_DO_WERYFIKACJI when performedBy == UserType.SPECIALIST => Success(),
            TicketStatus.OCZEKUJE_NA_ODPOWIEDZ when performedBy == UserType.SPECIALIST => Success(),
            TicketStatus.ESKALOWANE when performedBy == UserType.ADMINISTRATOR => Success(),
            _ => Failure($"Cannot transition from W_TOKU to {target}")
        };
    }

    private Result<bool> CanTransitionFromOczekujeNaOdpowiedz(TicketStatus target, UserType performedBy)
    {
        return target switch
        {
            TicketStatus.W_TOKU when performedBy == UserType.WORKER || performedBy == UserType.SPECIALIST => Success(),
            TicketStatus.ESKALOWANE when performedBy == UserType.ADMINISTRATOR => Success(),
            _ => Failure($"Cannot transition from OCZEKUJE_NA_ODPOWIEDZ to {target}")
        };
    }

    private Result<bool> CanTransitionFromGotoweDoWeryfikacji(TicketStatus target, UserType performedBy)
    {
        return target switch
        {
            TicketStatus.ZAMKNIETE when performedBy == UserType.WORKER => Success(),
            TicketStatus.ESKALOWANE when performedBy == UserType.WORKER => Success(),
            TicketStatus.W_TOKU when performedBy == UserType.ADMINISTRATOR => Success(),
            _ => Failure($"Cannot transition from GOTOWE_DO_WERYFIKACJI to {target}")
        };
    }

    private Result<bool> CanTransitionFromEskalowane(TicketStatus target, UserType performedBy)
    {
        return target switch
        {
            TicketStatus.W_TOKU when performedBy == UserType.ADMINISTRATOR => Success(),
            TicketStatus.PRZYPISANE when performedBy == UserType.ADMINISTRATOR => Success(),
            _ => Failure($"Cannot transition from ESKALOWANE to {target}")
        };
    }
}
