namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do eskalacji zgłoszenia.
/// </summary>
public class EscalateTicketRequest
{
    public string EscalationReason { get; set; } = string.Empty;
}
