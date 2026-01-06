namespace TicketingSystem.Domain.Enums;

/// <summary>
/// Status zgłoszenia w systemie.
/// </summary>
public enum TicketStatus
{
    NOWE,
    PRZYPISANE,
    W_TOKU,
    OCZEKUJE_NA_ODPOWIEDZ,
    GOTOWE_DO_WERYFIKACJI,
    ESKALOWANE,
    ZAMKNIETE
}
