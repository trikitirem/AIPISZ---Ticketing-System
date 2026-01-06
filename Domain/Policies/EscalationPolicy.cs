using TicketingSystem.Domain.Aggregates.Ticket;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Domain.Policies;

/// <summary>
/// Policy sprawdzająca czy zgłoszenie powinno być automatycznie eskalowane.
/// Auto-eskalacja przy SLA timeout lub 3+ nieudanych próbach.
/// </summary>
public class EscalationPolicy : Policy
{
    /// <summary>
    /// Sprawdza czy zgłoszenie powinno być automatycznie eskalowane.
    /// (np. jeśli SLA się mija lub wielokrotnie się nie udało)
    /// </summary>
    public Result<bool> ShouldAutoEscalateTicket(Ticket ticket, TicketStatus currentStatus, DateTime slaDeadline)
    {
        if (DateTime.UtcNow > slaDeadline && currentStatus == TicketStatus.W_TOKU)
        {
            return Success();
        }

        if (ticket.GetEscalationCount() >= 3)
        {
            return Success();
        }

        return Result<bool>.CreateSuccess(false);
    }
}
