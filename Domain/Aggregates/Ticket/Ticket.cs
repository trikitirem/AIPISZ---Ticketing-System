using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using TicketingSystem.Domain.Base;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Validators;
using TicketingSystem.Domain.ValueObjects;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.Aggregates.Ticket;

/// <summary>
/// Główny agregat reprezentujący zgłoszenie w systemie.
/// </summary>
public class Ticket : AggregateRoot
{
    public TicketNumber Number { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public TicketStatus Status { get; private set; }
    public Priority Priority { get; private set; }
    public TicketCategory Category { get; private set; }
    public string? AssignedTeamId { get; private set; }
    public string? AssignedSpecialistId { get; private set; }
    public string CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    private Resolution? _resolution;
    private Satisfaction? _satisfaction;
    private SLA _sla;

    private readonly List<Comment> _comments = new();
    private readonly List<Escalation> _escalations = new();
    private readonly List<Attachment> _attachments = new();
    private readonly List<HistoryChange> _history = new();

    private Ticket()
    {
        Number = TicketNumber.Create(string.Empty);
        Title = string.Empty;
        Description = string.Empty;
        Priority = Priority.Create(PriorityLevel.NISKI);
        CreatedById = string.Empty;
        _sla = SLA.Create(Priority);
    }

    private Ticket(string id, TicketNumber number, string title, string description, TicketCategory category, Priority priority, string createdById)
    {
        Id = id;
        Number = number;
        Title = title;
        Description = description;
        Category = category;
        Priority = priority;
        CreatedById = createdById;
        Status = TicketStatus.NOWE;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _sla = SLA.Create(priority);
    }

    public static Ticket Create(string id, string title, string description, TicketCategory category, PriorityLevel priorityLevel, string createdById)
    {
        var validator = new TicketValidator();
        var validationResult = validator.Validate((id, title, description, category, priorityLevel, createdById));

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new DomainExceptions.ValidationException("TICKET_DATA_VALIDATION_ERROR", errors);
        }

        var ticketNumber = TicketNumber.Create($"T-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}");
        var priority = Priority.Create(priorityLevel);

        var ticket = new Ticket(id, ticketNumber, title, description, category, priority, createdById);
        ticket.RecordChange("CREATED", null, TicketStatus.NOWE.ToString(), createdById, "Ticket created");

        return ticket;
    }

    public void ChangeStatus(TicketStatus newStatus)
    {
        if (Status == newStatus)
            return;

        var previousStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        if (newStatus == TicketStatus.ZAMKNIETE)
        {
            ResolvedAt = DateTime.UtcNow;
        }

        RecordChange("STATUS_CHANGED", previousStatus.ToString(), newStatus.ToString(), CreatedById, $"Status changed from {previousStatus} to {newStatus}");
    }

    public void AssignTo(string specialistId)
    {
        if (string.IsNullOrWhiteSpace(specialistId))
            throw new DomainExceptions.ValidationException("TICKET_ASSIGNMENT_DATA_VALIDATION_ERROR", "Specialist ID cannot be null or empty");

        AssignedSpecialistId = specialistId;
        UpdatedAt = DateTime.UtcNow;

        if (Status == TicketStatus.NOWE)
        {
            ChangeStatus(TicketStatus.PRZYPISANE);
        }

        RecordChange("ASSIGNED", null, specialistId, CreatedById, $"Ticket assigned to specialist {specialistId}");
    }

    public void AssignToTeam(string teamId)
    {
        if (string.IsNullOrWhiteSpace(teamId))
            throw new DomainExceptions.ValidationException("TICKET_ASSIGNMENT_DATA_VALIDATION_ERROR", "Team ID cannot be null or empty");

        AssignedTeamId = teamId;
        UpdatedAt = DateTime.UtcNow;

        if (Status == TicketStatus.NOWE)
        {
            ChangeStatus(TicketStatus.PRZYPISANE);
        }

        RecordChange("ASSIGNED_TO_TEAM", null, teamId, CreatedById, $"Ticket assigned to team {teamId}");
    }

    public void MarkAsReadyForVerification(string resolutionDescription, ResolutionType resolutionType)
    {
        if (Status != TicketStatus.W_TOKU)
            throw new DomainExceptions.ConflictException("TICKET_STATUS_TRANSITION_ERROR", $"Cannot mark as ready for verification. Current status is {Status}");

        if (string.IsNullOrWhiteSpace(resolutionDescription))
            throw new DomainExceptions.ValidationException("TICKET_RESOLUTION_DATA_VALIDATION_ERROR", "Resolution description cannot be null or empty");

        var resolutionResult = Resolution.Create(resolutionType, resolutionDescription);
        if (!resolutionResult.IsSuccess)
            throw new DomainExceptions.ValidationException("TICKET_RESOLUTION_DATA_VALIDATION_ERROR", resolutionResult.Error ?? "Cannot create resolution");

        _resolution = resolutionResult.Value;
        ChangeStatus(TicketStatus.GOTOWE_DO_WERYFIKACJI);
        UpdatedAt = DateTime.UtcNow;

        RecordChange("READY_FOR_VERIFICATION", TicketStatus.W_TOKU.ToString(), TicketStatus.GOTOWE_DO_WERYFIKACJI.ToString(), AssignedSpecialistId ?? CreatedById, $"Marked as ready for verification: {resolutionType}");
    }

    public void Escalate(string reason, Priority? newPriority = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainExceptions.ValidationException("TICKET_ESCALATION_DATA_VALIDATION_ERROR", "Escalation reason cannot be null or empty");

        var previousPriority = Priority;
        if (newPriority is not null)
        {
            Priority = newPriority;
            _sla = SLA.Create(newPriority);
        }

        var escalationResult = Escalation.Create(
            Guid.NewGuid().ToString(),
            reason,
            CreatedById,
            EscalationType.WORKER_INITIATED,
            previousPriority,
            newPriority
        );

        if (!escalationResult.IsSuccess)
            throw new DomainExceptions.ValidationException("TICKET_ESCALATION_DATA_VALIDATION_ERROR", escalationResult.Error ?? "Cannot create escalation");

        if (escalationResult.Value is null)
            throw new DomainExceptions.ValidationException("TICKET_ESCALATION_DATA_VALIDATION_ERROR", "Escalation value is null");

        _escalations.Add(escalationResult.Value);
        ChangeStatus(TicketStatus.ESKALOWANE);
        UpdatedAt = DateTime.UtcNow;

        RecordChange("ESCALATED", Status.ToString(), TicketStatus.ESKALOWANE.ToString(), CreatedById, $"Escalated: {reason}");
    }

    public void AddComment(string authorId, string content, bool isInternal)
    {
        if (string.IsNullOrWhiteSpace(authorId))
            throw new DomainExceptions.ValidationException("TICKET_COMMENT_DATA_VALIDATION_ERROR", "Author ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(content))
            throw new DomainExceptions.ValidationException("TICKET_COMMENT_DATA_VALIDATION_ERROR", "Content cannot be null or empty");

        var comment = Comment.Create(Guid.NewGuid().ToString(), authorId, content, isInternal);
        _comments.Add(comment);
        UpdatedAt = DateTime.UtcNow;

        RecordChange("COMMENT_ADDED", null, content, authorId, $"Comment added (Internal: {isInternal})");
    }

    public void AddAttachment(Attachment attachment)
    {
        if (attachment is null)
            throw new DomainExceptions.ValidationException("TICKET_ATTACHMENT_DATA_VALIDATION_ERROR", "Attachment cannot be null");

        _attachments.Add(attachment);
        UpdatedAt = DateTime.UtcNow;

        RecordChange("ATTACHMENT_ADDED", null, attachment.FileName, CreatedById, $"Attachment added: {attachment.FileName}");
    }

    public void AddEscalation(Escalation escalation)
    {
        if (escalation is null)
            throw new DomainExceptions.ValidationException("TICKET_ESCALATION_DATA_VALIDATION_ERROR", "Escalation cannot be null");

        _escalations.Add(escalation);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordChange(string changeType, string? previousValue, string? newValue, string performedBy, string? description = null)
    {
        var historyChange = HistoryChange.Create(
            Guid.NewGuid().ToString(),
            changeType,
            previousValue,
            newValue,
            performedBy,
            description
        );

        _history.Add(historyChange);
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Id) &&
               !string.IsNullOrWhiteSpace(Title) &&
               !string.IsNullOrWhiteSpace(Description) &&
               !string.IsNullOrWhiteSpace(CreatedById);
    }

    public IReadOnlyList<Comment> GetComments()
    {
        return _comments.AsReadOnly();
    }

    public int GetEscalationCount()
    {
        return _escalations.Count;
    }

    public bool WasReproducedBefore()
    {
        return _comments.Any(c => !c.IsInternal && c.Content.Contains("reproduce", StringComparison.OrdinalIgnoreCase)) ||
               _history.Any(h => h.ChangeType.Contains("REPRODUCED", StringComparison.OrdinalIgnoreCase));
    }

    public void RecordSatisfaction(int rating, string? comment, bool isProblemResolved)
    {
        if (rating < 1 || rating > 5)
            throw new DomainExceptions.ValidationException("TICKET_SATISFACTION_DATA_VALIDATION_ERROR", "Rating must be between 1 and 5");

        _satisfaction = Satisfaction.Create(Guid.NewGuid().ToString(), rating, comment, isProblemResolved);
        UpdatedAt = DateTime.UtcNow;

        RecordChange("SATISFACTION_RECORDED", null, rating.ToString(), CreatedById, $"Satisfaction recorded: {rating} stars, Problem resolved: {isProblemResolved}");
    }

    public override Dictionary<string, object> ToPrimitive()
    {
        var dict = new Dictionary<string, object>
        {
            { "Id", Id },
            { "Number", Number.Value },
            { "Title", Title },
            { "Description", Description },
            { "Status", Status.ToString() },
            { "Priority", Priority.Level.ToString() },
            { "Category", Category.ToString() },
            { "CreatedById", CreatedById },
            { "CreatedAt", CreatedAt },
            { "UpdatedAt", UpdatedAt }
        };

        if (AssignedTeamId is not null)
            dict["AssignedTeamId"] = AssignedTeamId;

        if (AssignedSpecialistId is not null)
            dict["AssignedSpecialistId"] = AssignedSpecialistId;

        if (ResolvedAt.HasValue)
            dict["ResolvedAt"] = ResolvedAt.Value;

        return dict;
    }
}
