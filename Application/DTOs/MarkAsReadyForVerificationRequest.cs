namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do oznaczenia zgłoszenia jako gotowego do weryfikacji.
/// </summary>
public class MarkAsReadyForVerificationRequest
{
    public string ResolutionDescription { get; set; } = string.Empty;
    public string ResolutionType { get; set; } = string.Empty;
}
