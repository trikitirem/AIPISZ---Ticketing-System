namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Response DTO dla zmiany w historii.
/// </summary>
public class HistoryChangeDTO
{
    public string Id { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public string ChangeType { get; set; } = string.Empty;
    public string? PreviousValue { get; set; }
    public string? NewValue { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public string? Description { get; set; }
}
