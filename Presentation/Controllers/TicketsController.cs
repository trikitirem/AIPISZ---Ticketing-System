using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Application.Mappers;
using TicketingSystem.Application.Services;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Presentation.Controllers;

/// <summary>
/// Kontroler do zarządzania zgłoszeniami.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly TicketService _ticketService;
    private readonly TicketMapper _ticketMapper;

    public TicketsController(TicketService ticketService, TicketMapper ticketMapper)
    {
        _ticketService = ticketService;
        _ticketMapper = ticketMapper;
    }

    [HttpPost]
    public async Task<ActionResult<TicketDTO>> CreateTicket([FromBody] CreateTicketRequest request, [FromQuery] string createdById)
    {
        var category = Enum.Parse<TicketCategory>(request.Category, true);
        var priority = Enum.Parse<PriorityLevel>(request.Priority, true);
        var ticketId = Guid.NewGuid().ToString();

        var ticket = await _ticketService.CreateTicketAsync(ticketId, request.Title, request.Description, category, priority, createdById);
        var dto = _ticketMapper.Map(ticket);

        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TicketDetailDTO>> GetTicket(string id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        var dto = _ticketMapper.MapDetail(ticket);

        return Ok(dto);
    }

    [HttpPost("{id}/mark-ready-for-verification")]
    public async Task<IActionResult> MarkAsReadyForVerification(string id, [FromBody] MarkAsReadyForVerificationRequest request, [FromQuery] string specialistId)
    {
        var resolutionType = Enum.Parse<ResolutionType>(request.ResolutionType, true);
        await _ticketService.MarkAsReadyForVerificationAsync(id, specialistId, request.ResolutionDescription, resolutionType);

        return NoContent();
    }

    [HttpPost("{id}/review-resolution")]
    public async Task<IActionResult> ReviewResolution(string id, [FromBody] ReviewResolutionRequest request, [FromQuery] string workerId)
    {
        await _ticketService.ReviewResolutionAsync(id, workerId, request.Accepted, request.ReviewComment);

        return NoContent();
    }

    [HttpPost("{id}/escalate")]
    public async Task<IActionResult> EscalateTicket(string id, [FromBody] EscalateTicketRequest request, [FromQuery] string escalatedBy)
    {
        await _ticketService.EscalateTicketAsync(id, escalatedBy, request.EscalationReason);

        return NoContent();
    }

    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddComment(string id, [FromBody] AddCommentRequest request, [FromQuery] string authorId)
    {
        await _ticketService.AddCommentAsync(id, authorId, request.Content, request.IsInternal);

        return NoContent();
    }

    [HttpPost("{id}/attachments")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<AttachmentDTO>> UploadAttachment(string id, [FromForm] UploadAttachmentRequest request, [FromQuery] string uploadedBy)
    {
        if (request?.File is null || request.File.Length == 0)
        {
            return BadRequest("File is required");
        }

        var file = request.File;
        using var stream = file.OpenReadStream();
        var attachment = await _ticketService.UploadAttachmentAsync(id, uploadedBy, file.FileName, file.Length, file.ContentType, stream);

        var dto = new AttachmentDTO
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            FileSize = attachment.FileSize,
            MimeType = attachment.MimeType,
            UploadedAt = attachment.UploadedAt,
            UploadedBy = attachment.UploadedBy
        };

        return Ok(dto);
    }

    [HttpPut("{id}/assign")]
    public async Task<IActionResult> AssignTicket(string id, [FromBody] AssignTicketRequest request)
    {
        await _ticketService.AssignTicketAsync(id, request.SpecialistId, request.TeamId);

        return NoContent();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> ChangeStatus(string id, [FromBody] ChangeTicketStatusRequest request, [FromQuery] string performedBy)
    {
        var status = Enum.Parse<TicketStatus>(request.Status, true);
        await _ticketService.ChangeTicketStatusAsync(id, status, performedBy);

        return NoContent();
    }
}
