namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do dodania komentarza do zgłoszenia.
/// </summary>
public class AddCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
}
