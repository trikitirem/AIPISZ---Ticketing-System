using TicketingSystem.Application.Mappers;
using TicketingSystem.Domain.Aggregates.Ticket;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Exceptions;
using TicketingSystem.Domain.Policies;
using TicketingSystem.Domain.ValueObjects;
using TicketingSystem.Infrastructure.Persistence;

namespace TicketingSystem.Application.Services;

/// <summary>
/// Service do zarządzania zgłoszeniami.
/// Orchestruje operacje między Repositories, Policies i Domain Models.
/// </summary>
public class TicketService
{
    private readonly TicketRepository _ticketRepository;
    private readonly UserRepository _userRepository;
    private readonly TeamRepository _teamRepository;
    private readonly ResolutionPolicy _resolutionPolicy;
    private readonly EscalationPolicy _escalationPolicy;
    private readonly WorkerEscalationPolicy _workerEscalationPolicy;
    private readonly SpecialistResolutionPolicy _specialistResolutionPolicy;
    private readonly AttachmentRepository _attachmentRepository;
    private readonly TicketMapper _ticketMapper;

    public TicketService(
        TicketRepository ticketRepository,
        UserRepository userRepository,
        TeamRepository teamRepository,
        ResolutionPolicy resolutionPolicy,
        EscalationPolicy escalationPolicy,
        WorkerEscalationPolicy workerEscalationPolicy,
        SpecialistResolutionPolicy specialistResolutionPolicy,
        AttachmentRepository attachmentRepository,
        TicketMapper ticketMapper)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _resolutionPolicy = resolutionPolicy;
        _escalationPolicy = escalationPolicy;
        _workerEscalationPolicy = workerEscalationPolicy;
        _specialistResolutionPolicy = specialistResolutionPolicy;
        _attachmentRepository = attachmentRepository;
        _ticketMapper = ticketMapper;
    }

    public async Task<Ticket> CreateTicketAsync(string id, string title, string description, TicketCategory category, PriorityLevel priorityLevel, string createdById)
    {
        var ticket = Ticket.Create(id, title, description, category, priorityLevel, createdById);
        await _ticketRepository.SaveAsync(ticket);
        return ticket;
    }

    public async Task<Ticket> GetTicketByIdAsync(string ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found", ticketId);
        }
        return ticket;
    }

    public async Task MarkAsReadyForVerificationAsync(string ticketId, string specialistId, string resolutionDescription, ResolutionType resolutionType)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found", ticketId);
        }

        var specialist = await _userRepository.GetByIdAsync(specialistId);
        if (specialist is not Domain.Aggregates.User.SupportSpecialist supportSpecialist)
        {
            throw new ValidationException("User is not a specialist", specialistId);
        }

        var resolutionResult = Resolution.Create(resolutionType, resolutionDescription);
        if (!resolutionResult.IsSuccess || resolutionResult.Value is null)
        {
            throw new ValidationException("Invalid resolution", resolutionResult.Error ?? "Cannot create resolution");
        }

        var policyResult = _specialistResolutionPolicy.CanMarkAsReadyForVerification(ticket, supportSpecialist, resolutionResult.Value);
        if (!policyResult.IsSuccess)
        {
            throw new ConflictException("Cannot mark as ready", policyResult.Error ?? "Policy validation failed");
        }

        ticket.MarkAsReadyForVerification(resolutionDescription, resolutionType);
        await _ticketRepository.SaveAsync(ticket);
    }

    public async Task ReviewResolutionAsync(string ticketId, string workerId, bool accepted, string reviewComment)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found", ticketId);
        }

        if (ticket.Status != TicketStatus.GOTOWE_DO_WERYFIKACJI)
        {
            throw new ConflictException("Ticket is not ready for verification", ticketId);
        }

        var worker = await _userRepository.GetByIdAsync(workerId);
        if (worker is not Domain.Aggregates.User.Worker workerUser)
        {
            throw new ValidationException("User is not a worker", workerId);
        }

        if (accepted)
        {
            ticket.ChangeStatus(TicketStatus.ZAMKNIETE);
            ticket.RecordSatisfaction(0, reviewComment, true);
        }
        else
        {
            await EscalateTicketAsync(ticketId, workerId, reviewComment);
        }

        await _ticketRepository.SaveAsync(ticket);
    }

    public async Task EscalateTicketAsync(string ticketId, string escalatedBy, string escalationReason)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found", ticketId);
        }

        var worker = await _userRepository.GetByIdAsync(escalatedBy);
        if (worker is not Domain.Aggregates.User.Worker workerUser)
        {
            throw new ValidationException("User is not a worker", escalatedBy);
        }

        var policyResult = _workerEscalationPolicy.CanWorkerEscalate(ticket, workerUser, escalationReason);
        if (!policyResult.IsSuccess)
        {
            throw new ForbiddenException("Cannot escalate ticket", policyResult.Error ?? "Policy validation failed");
        }

        var escalationResult = Escalation.Create(
            Guid.NewGuid().ToString(),
            escalationReason,
            escalatedBy,
            EscalationType.WORKER_INITIATED,
            ticket.Priority,
            null
        );

        if (!escalationResult.IsSuccess || escalationResult.Value is null)
        {
            throw new ValidationException("Cannot create escalation", escalationResult.Error ?? "Escalation creation failed");
        }

        ticket.Escalate(escalationReason);
        ticket.AddEscalation(escalationResult.Value);
        await _ticketRepository.SaveAsync(ticket);
    }

    public async Task AddCommentAsync(string ticketId, string authorId, string content, bool isInternal)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found", ticketId);
        }

        ticket.AddComment(authorId, content, isInternal);
        await _ticketRepository.SaveAsync(ticket);
    }

    public async Task<Attachment> UploadAttachmentAsync(string ticketId, string uploadedBy, string fileName, long fileSize, string mimeType, Stream fileStream)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found", ticketId);
        }

        var attachment = Attachment.Create(Guid.NewGuid().ToString(), fileName, fileSize, mimeType, uploadedBy);
        ticket.AddAttachment(attachment);
        await _attachmentRepository.SaveFileAsync(attachment, fileStream);
        await _ticketRepository.SaveAsync(ticket);

        return attachment;
    }

    public async Task AssignTicketAsync(string ticketId, string? specialistId, string? teamId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found", ticketId);
        }

        if (!string.IsNullOrWhiteSpace(specialistId))
        {
            var specialist = await _userRepository.GetByIdAsync(specialistId);
            if (specialist is not Domain.Aggregates.User.SupportSpecialist)
            {
                throw new ValidationException("User is not a specialist", specialistId);
            }

            ticket.AssignTo(specialistId);
        }
        else if (!string.IsNullOrWhiteSpace(teamId))
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team is null)
            {
                throw new NotFoundException("Team not found", teamId);
            }

            ticket.AssignToTeam(teamId);
        }
        else
        {
            throw new ValidationException("Either specialistId or teamId must be provided", string.Empty);
        }

        await _ticketRepository.SaveAsync(ticket);
    }

    public async Task ChangeTicketStatusAsync(string ticketId, TicketStatus newStatus, string performedBy)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket is null)
        {
            throw new NotFoundException("Ticket not found", ticketId);
        }

        var user = await _userRepository.GetByIdAsync(performedBy);
        if (user is null)
        {
            throw new NotFoundException("User not found", performedBy);
        }

        var statusPolicy = new TicketStatusPolicy();
        var policyResult = statusPolicy.CanTransitionTo(ticket.Status, newStatus, user.GetUserType());
        if (!policyResult.IsSuccess)
        {
            throw new ConflictException("Cannot change status", policyResult.Error ?? "Status transition not allowed");
        }

        ticket.ChangeStatus(newStatus);
        await _ticketRepository.SaveAsync(ticket);
    }
}
