using FluentValidation;

namespace TicketingSystem.Domain.Validators;

public class HistoryChangeValidator : AbstractValidator<(string id, string changeType, string performedBy)>
{
    public HistoryChangeValidator()
    {
        RuleFor(x => x.id)
            .NotEmpty().WithMessage("HistoryChange ID cannot be empty")
            .NotNull().WithMessage("HistoryChange ID cannot be null");

        RuleFor(x => x.changeType)
            .NotEmpty().WithMessage("ChangeType cannot be empty")
            .NotNull().WithMessage("ChangeType cannot be null");

        RuleFor(x => x.performedBy)
            .NotEmpty().WithMessage("PerformedBy cannot be empty")
            .NotNull().WithMessage("PerformedBy cannot be null");
    }
}
