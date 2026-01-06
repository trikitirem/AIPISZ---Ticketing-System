namespace TicketingSystem.Domain.Enums;

/// <summary>
/// Typ eskalacji zgłoszenia.
/// </summary>
public enum EscalationType
{
    WORKER_INITIATED,
    SLA_TIMEOUT,
    AUTO_ESCALATION,
    ADMIN_INITIATED
}
