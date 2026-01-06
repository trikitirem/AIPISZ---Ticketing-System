namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Response DTO dla zgłoszenia ze wszystkimi szczegółami.
/// </summary>
public class TicketDetailDTO : TicketDTO
{
    public string Description { get; set; } = string.Empty;
    public List<CommentDTO> Comments { get; set; } = new();
    public List<AttachmentDTO> Attachments { get; set; } = new();
    public List<HistoryChangeDTO> History { get; set; } = new();
    public List<EscalationDTO> Escalations { get; set; } = new();
    public ResolutionDTO? Resolution { get; set; }
}
