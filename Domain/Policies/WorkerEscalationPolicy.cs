using TicketingSystem.Domain.Aggregates.Ticket;
using TicketingSystem.Domain.Aggregates.User;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Domain.Policies;

/// <summary>
/// Policy sprawdzająca czy Worker może eskalować zgłoszenie.
/// Worker może eskalować TYLKO ze statusu GOTOWE_DO_WERYFIKACJI.
/// Tylko creator ticketu może eskalować.
/// Wymaga powodu eskalacji.
/// </summary>
public class WorkerEscalationPolicy : Policy
{
    /// <summary>
    /// Sprawdza czy Worker może eskalować zgłoszenie.
    /// Worker może eskalować TYLKO ze statusu GOTOWE_DO_WERYFIKACJI.
    /// </summary>
    public Result<bool> CanWorkerEscalate(Ticket ticket, Worker worker, string escalationReason)
    {
        if (ticket.Status != TicketStatus.GOTOWE_DO_WERYFIKACJI)
        {
            return Failure($"Can only escalate from GOTOWE_DO_WERYFIKACJI status, current status is {ticket.Status}");
        }

        if (ticket.CreatedById != worker.Id)
        {
            return Failure("Only ticket creator can escalate");
        }

        if (string.IsNullOrWhiteSpace(escalationReason))
        {
            return Failure("Escalation reason is required");
        }

        return Success();
    }
}
