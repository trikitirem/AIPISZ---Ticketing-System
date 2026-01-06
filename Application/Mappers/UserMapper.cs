using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Aggregates.User;

namespace TicketingSystem.Application.Mappers;

/// <summary>
/// Mapper do konwersji User na DTOs.
/// </summary>
public class UserMapper
{
    public UserDTO Map(User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Email = user.GetEmail(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserType = user.GetUserType().ToString(),
            AccountStatus = user.AccountStatus.Status.ToString()
        };
    }

    public SupportSpecialistDTO MapSpecialist(SupportSpecialist specialist)
    {
        return new SupportSpecialistDTO
        {
            Id = specialist.Id,
            Email = specialist.GetEmail(),
            FirstName = specialist.FirstName,
            LastName = specialist.LastName,
            UserType = specialist.GetUserType().ToString(),
            AccountStatus = specialist.AccountStatus.Status.ToString(),
            TeamId = specialist.TeamId,
            ActiveTicketCount = specialist.CurrentActiveCount,
            ActiveTicketLimit = specialist.ActiveTicketLimit
        };
    }

    public List<UserDTO> MapList(IEnumerable<User> users)
    {
        return users.Select(Map).ToList();
    }
}
