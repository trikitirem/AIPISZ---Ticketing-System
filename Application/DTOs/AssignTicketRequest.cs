namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do przypisania zgłoszenia.
/// </summary>
public class AssignTicketRequest
{
    public string? SpecialistId { get; set; }
    public string? TeamId { get; set; }
}
