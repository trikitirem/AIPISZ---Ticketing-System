namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do tworzenia zespołu.
/// </summary>
public class CreateTeamRequest
{
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int MaxTickets { get; set; } = 50;
}
