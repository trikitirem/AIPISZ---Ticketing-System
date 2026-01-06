using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Validators;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.Aggregates.Ticket;

/// <summary>
/// Załącznik do zgłoszenia.
/// </summary>
public class Attachment : Entity<string>
{
    public string FileName { get; private set; }
    public long FileSize { get; private set; }
    public string MimeType { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public string UploadedBy { get; private set; }

    private Attachment()
    {
        FileName = string.Empty;
        MimeType = string.Empty;
        UploadedBy = string.Empty;
    }

    private Attachment(string id, string fileName, long fileSize, string mimeType, string uploadedBy)
    {
        Id = id;
        FileName = fileName;
        FileSize = fileSize;
        MimeType = mimeType;
        UploadedBy = uploadedBy;
        UploadedAt = DateTime.UtcNow;
    }

    public static Attachment Create(string id, string fileName, long fileSize, string mimeType, string uploadedBy)
    {
        var validator = new AttachmentValidator();
        var validationResult = validator.Validate((id, fileName, fileSize, mimeType, uploadedBy));

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainExceptions.ValidationException("ATTACHMENT_DATA_VALIDATION_ERROR", errors);
        }

        return new Attachment(id, fileName, fileSize, mimeType, uploadedBy);
    }

    public string GetStoragePath()
    {
        return $"Data/uploads/{Id}/{FileName}";
    }

    public override Dictionary<string, object> ToPrimitive()
    {
        return new Dictionary<string, object>
        {
            { "Id", Id },
            { "FileName", FileName },
            { "FileSize", FileSize },
            { "MimeType", MimeType },
            { "UploadedAt", UploadedAt },
            { "UploadedBy", UploadedBy }
        };
    }
}
