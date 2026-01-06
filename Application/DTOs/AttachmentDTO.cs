namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Response DTO dla załącznika.
/// </summary>
public class AttachmentDTO
{
    public string Id { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
}
