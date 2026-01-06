namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Response DTO dla komentarza.
/// </summary>
public class CommentDTO
{
    public string Id { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTime CreatedAt { get; set; }
}
