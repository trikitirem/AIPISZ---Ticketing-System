namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Response DTO dla zespołu.
/// </summary>
public class TeamDTO
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int SpecialistCount { get; set; }
}
