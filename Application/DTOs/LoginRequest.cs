namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do logowania użytkownika.
/// </summary>
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
}
