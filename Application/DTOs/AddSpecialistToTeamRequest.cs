namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do dodania specjalisty do zespołu.
/// </summary>
public class AddSpecialistToTeamRequest
{
    public string SpecialistId { get; set; } = string.Empty;
}
