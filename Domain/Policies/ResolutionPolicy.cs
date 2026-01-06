using TicketingSystem.Domain.Aggregates.Ticket;
using TicketingSystem.Domain.Aggregates.User;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Domain.Policies;

/// <summary>
/// Policy sprawdzająca czy rozwiązanie jest wiarygodne.
/// Zapobiega "NIE_MOZNA_ODTWORZYC" dla łatwo odtwarzalnych problemów.
/// </summary>
public class ResolutionPolicy : Policy
{
    /// <summary>
    /// Sprawdza czy resolution jest wiarygodne.
    /// Zapobiega "NIE_MOŻNA_ODTWORZYĆ" dla łatwo odtwarzalnych problemów.
    /// </summary>
    public Result<bool> CanAcceptResolution(Ticket ticket, Resolution resolution, SupportSpecialist specialist)
    {
        if (resolution.Type == ResolutionType.NIE_MOZNA_ODTWORZYC)
        {
            if (ticket.WasReproducedBefore())
            {
                return Failure("Cannot mark as NOT_REPRODUCIBLE - problem was reproducible before");
            }
        }

        return Success();
    }
}
