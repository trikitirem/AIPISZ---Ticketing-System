namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do zmiany statusu zgłoszenia.
/// </summary>
public class ChangeTicketStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
