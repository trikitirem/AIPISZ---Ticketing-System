using System.Reflection;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Aggregates.Ticket;

namespace TicketingSystem.Application.Mappers;

/// <summary>
/// Mapper do konwersji Ticket na DTOs.
/// </summary>
public class TicketMapper
{
    private readonly CommentMapper _commentMapper;

    public TicketMapper()
    {
        _commentMapper = new CommentMapper();
    }

    public TicketDTO Map(Ticket ticket)
    {
        return new TicketDTO
        {
            Id = ticket.Id,
            Number = ticket.Number.Value,
            Title = ticket.Title,
            Status = ticket.Status.ToString(),
            Priority = ticket.Priority.Level.ToString(),
            Category = ticket.Category.ToString(),
            AssignedSpecialistId = ticket.AssignedSpecialistId,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }

    public TicketDetailDTO MapDetail(Ticket ticket, IReadOnlyList<Comment>? comments = null)
    {
        var dto = new TicketDetailDTO
        {
            Id = ticket.Id,
            Number = ticket.Number.Value,
            Title = ticket.Title,
            Status = ticket.Status.ToString(),
            Priority = ticket.Priority.Level.ToString(),
            Category = ticket.Category.ToString(),
            AssignedSpecialistId = ticket.AssignedSpecialistId,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            Description = ticket.Description
        };

        dto.Comments = (comments ?? ticket.GetComments())
            .Select(c => _commentMapper.Map(c))
            .ToList();

        dto.Attachments = GetAttachments(ticket)
            .Select(a => new AttachmentDTO
            {
                Id = a.Id,
                FileName = a.FileName,
                FileSize = a.FileSize,
                MimeType = a.MimeType,
                UploadedAt = a.UploadedAt,
                UploadedBy = a.UploadedBy
            })
            .ToList();

        dto.History = GetHistory(ticket)
            .Select(h => new HistoryChangeDTO
            {
                Id = h.Id,
                ChangedAt = h.ChangedAt,
                ChangeType = h.ChangeType,
                PreviousValue = h.PreviousValue,
                NewValue = h.NewValue,
                PerformedBy = h.PerformedBy,
                Description = h.Description
            })
            .ToList();

        dto.Escalations = GetEscalations(ticket)
            .Select(e => new EscalationDTO
            {
                Id = e.Id,
                Reason = e.Reason,
                EscalatedBy = e.EscalatedBy,
                EscalationType = e.EscalationType.ToString(),
                CreatedAt = e.EscalatedAt
            })
            .ToList();

        var resolution = GetResolution(ticket);
        if (resolution is not null)
        {
            dto.Resolution = new ResolutionDTO
            {
                Type = resolution.Type.ToString(),
                Description = resolution.Description,
                Tags = resolution.Tags,
                CreatedAt = resolution.CreatedAt
            };
        }

        return dto;
    }

    public List<TicketDTO> MapList(IEnumerable<Ticket> tickets)
    {
        return tickets.Select(Map).ToList();
    }

    private static List<Attachment> GetAttachments(Ticket ticket)
    {
        var field = typeof(Ticket).GetField("_attachments", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field?.GetValue(ticket) is List<Attachment> attachments)
        {
            return attachments;
        }
        return new List<Attachment>();
    }

    private static List<HistoryChange> GetHistory(Ticket ticket)
    {
        var field = typeof(Ticket).GetField("_history", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field?.GetValue(ticket) is List<HistoryChange> history)
        {
            return history;
        }
        return new List<HistoryChange>();
    }

    private static List<Escalation> GetEscalations(Ticket ticket)
    {
        var field = typeof(Ticket).GetField("_escalations", BindingFlags.NonPublic | BindingFlags.Instance);
        if (field?.GetValue(ticket) is List<Escalation> escalations)
        {
            return escalations;
        }
        return new List<Escalation>();
    }

    private static Resolution? GetResolution(Ticket ticket)
    {
        var field = typeof(Ticket).GetField("_resolution", BindingFlags.NonPublic | BindingFlags.Instance);
        return field?.GetValue(ticket) as Resolution;
    }
}
