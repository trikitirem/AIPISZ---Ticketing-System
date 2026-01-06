namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Request DTO do rejestracji użytkownika.
/// </summary>
public class RegisterUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
}
