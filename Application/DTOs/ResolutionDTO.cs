namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Response DTO dla rozwiązania.
/// </summary>
public class ResolutionDTO
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
