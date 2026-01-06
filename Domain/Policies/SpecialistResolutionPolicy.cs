using TicketingSystem.Domain.Aggregates.Ticket;
using TicketingSystem.Domain.Aggregates.User;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Domain.Policies;

/// <summary>
/// Policy sprawdzająca czy Specialist może oznaczyć ticket jako GOTOWE_DO_WERYFIKACJI.
/// Zapobiega wychodzeniu z błędnymi rozwiązaniami.
/// </summary>
public class SpecialistResolutionPolicy : Policy
{
    private readonly ResolutionPolicy _resolutionPolicy;

    public SpecialistResolutionPolicy()
    {
        _resolutionPolicy = new ResolutionPolicy();
    }

    /// <summary>
    /// Sprawdza czy Specialist może oznaczyć ticket jako GOTOWE_DO_WERYFIKACJI.
    /// Zapobiega wychodzeniu z błędnymi rozwiązaniami.
    /// </summary>
    public Result<bool> CanMarkAsReadyForVerification(Ticket ticket, SupportSpecialist specialist, Resolution resolution)
    {
        if (ticket.AssignedSpecialistId != specialist.Id)
        {
            return Failure("Cannot mark ticket as ready - not assigned to you");
        }

        if (ticket.Status != TicketStatus.W_TOKU)
        {
            return Failure($"Can only mark as ready from W_TOKU status, current is {ticket.Status}");
        }

        var resolutionResult = _resolutionPolicy.CanAcceptResolution(ticket, resolution, specialist);

        if (!resolutionResult.IsSuccess)
        {
            return Failure(resolutionResult.Error);
        }

        return Success();
    }
}
