namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do przeglądu rozwiązania przez workera.
/// </summary>
public class ReviewResolutionRequest
{
    public bool Accepted { get; set; }
    public string ReviewComment { get; set; } = string.Empty;
}
