using FluentValidation;

namespace TicketingSystem.Domain.Validators;

public class AttachmentValidator : AbstractValidator<(string id, string fileName, long fileSize, string mimeType, string uploadedBy)>
{
    public AttachmentValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("Attachment ID cannot be empty")
            .NotNull().WithMessage("Attachment ID cannot be null");

        RuleFor(x => x.fileName)
            .NotEmpty().WithMessage("File name cannot be empty")
            .NotNull().WithMessage("File name cannot be null");

        RuleFor(x => x.fileSize)
            .GreaterThan(0).WithMessage("File size must be greater than zero");

        RuleFor(x => x.mimeType)
            .NotEmpty().WithMessage("MIME type cannot be empty")
            .NotNull().WithMessage("MIME type cannot be null");

        RuleFor(x => x.uploadedBy)
            .NotEmpty().WithMessage("UploadedBy cannot be empty")
            .NotNull().WithMessage("UploadedBy cannot be null");
    }
}
