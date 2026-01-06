namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Response DTO dla eskalacji.
/// </summary>
public class EscalationDTO
{
    public string Id { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string EscalatedBy { get; set; } = string.Empty;
    public string EscalationType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
