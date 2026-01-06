namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Response DTO dla specjalisty supportu.
/// </summary>
public class SupportSpecialistDTO : UserDTO
{
    public string? TeamId { get; set; }
    public int ActiveTicketCount { get; set; }
    public int ActiveTicketLimit { get; set; }
}
