namespace TicketingSystem.Application.DTOs;

/// <summary>
/// Response DTO dla użytkownika.
/// </summary>
public class UserDTO
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public string AccountStatus { get; set; } = string.Empty;
}
