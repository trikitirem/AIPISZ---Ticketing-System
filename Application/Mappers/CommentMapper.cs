using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Aggregates.Ticket;

namespace TicketingSystem.Application.Mappers;

/// <summary>
/// Mapper do konwersji Comment na DTOs.
/// </summary>
public class CommentMapper
{
    public CommentDTO Map(Comment comment)
    {
        return new CommentDTO
        {
            Id = comment.Id,
            AuthorId = comment.AuthorId,
            Content = comment.Content,
            IsInternal = comment.IsInternal,
            CreatedAt = comment.CreatedAt
        };
    }

    public List<CommentDTO> MapList(IEnumerable<Comment> comments)
    {
        return comments.Select(Map).ToList();
    }
}
