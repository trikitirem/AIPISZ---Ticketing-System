using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Validators;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.Aggregates.Ticket;

/// <summary>
/// Komentarz do zgłoszenia.
/// </summary>
public class Comment : Entity
{
    public string AuthorId { get; private set; }
    public string Content { get; private set; }
    public bool IsInternal { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Comment()
    {
        AuthorId = string.Empty;
        Content = string.Empty;
    }

    private Comment(string id, string authorId, string content, bool isInternal, DateTime createdAt)
    {
        Id = id;
        AuthorId = authorId;
        Content = content;
        IsInternal = isInternal;
        CreatedAt = createdAt;
    }

    public static Comment Create(string id, string authorId, string content, bool isInternal)
    {
        var validator = new CommentValidator();
        var validationResult = validator.Validate((id, authorId, content));

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainExceptions.ValidationException("COMMENT_DATA_VALIDATION_ERROR", errors);
        }

        return new Comment(id, authorId, content, isInternal, DateTime.UtcNow);
    }

    public override Dictionary<string, object> ToPrimitive()
    {
        return new Dictionary<string, object>
        {
            { "Id", Id },
            { "AuthorId", AuthorId },
            { "Content", Content },
            { "IsInternal", IsInternal },
            { "CreatedAt", CreatedAt }
        };
    }
}
