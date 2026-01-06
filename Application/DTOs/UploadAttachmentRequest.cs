using Microsoft.AspNetCore.Http;

namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do przesłania załącznika.
/// </summary>
public class UploadAttachmentRequest
{
    public IFormFile File { get; set; } = null!;
}
