namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Response DTO dla zgłoszenia (podstawowe informacje).
/// </summary>
public class TicketDTO
{
    public string Id { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? AssignedSpecialistId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
