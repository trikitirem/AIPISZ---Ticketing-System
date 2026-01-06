namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do tworzenia nowego zgłoszenia.
/// </summary>
public class CreateTicketRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
}
