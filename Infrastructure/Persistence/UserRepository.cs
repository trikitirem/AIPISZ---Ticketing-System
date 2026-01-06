using System.Reflection;
using Microsoft.Extensions.Logging;
using TicketingSystem.Domain.Aggregates.User;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Infrastructure.Persistence;

/// <summary>
/// Repozytorium dla agregatu User oparte na plikach JSON.
/// </summary>
public class UserRepository : FileBasedRepository<User, string>
{
    public UserRepository(ILogger<UserRepository> logger)
        : base("Data/users.json", logger)
    {
    }

    protected override User? FromPrimitive(Dictionary<string, object> data)
    {
        try
        {
            var id = data["Id"].ToString()!;
            var email = data["Email"].ToString()!;
            var firstName = data["FirstName"].ToString()!;
            var lastName = data["LastName"].ToString()!;
            var accountStatusEnum = Enum.Parse<AccountStatusEnum>(data["AccountStatus"].ToString()!);
            var userType = Enum.Parse<UserType>(data["UserType"].ToString()!);

            User user = userType switch
            {
                UserType.SPECIALIST => CreateSupportSpecialist(id, email, firstName, lastName, accountStatusEnum, data),
                UserType.ADMINISTRATOR => Administrator.Create(id, email, firstName, lastName, accountStatusEnum),
                UserType.WORKER => Worker.Create(id, email, firstName, lastName, accountStatusEnum),
                _ => throw new ArgumentException($"Unknown user type: {userType}")
            };

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing User from primitive data");
            return null;
        }
    }

    private static SupportSpecialist CreateSupportSpecialist(string id, string email, string firstName, string lastName, AccountStatusEnum accountStatus, Dictionary<string, object> data)
    {
        string? teamId = null;
        string? specialization = null;
        int activeTicketLimit = 10;

        if (data.TryGetValue("TeamId", out var teamIdObj) && teamIdObj is not null && teamIdObj.ToString() != string.Empty)
        {
            teamId = teamIdObj.ToString();
        }

        if (data.TryGetValue("Specialization", out var specializationObj) && specializationObj is not null && specializationObj.ToString() != string.Empty)
        {
            specialization = specializationObj.ToString();
        }

        if (data.TryGetValue("ActiveTicketLimit", out var limitObj))
        {
            activeTicketLimit = Convert.ToInt32(limitObj);
        }

        var specialist = SupportSpecialist.Create(id, email, firstName, lastName, accountStatus, teamId, specialization, activeTicketLimit);

        if (data.TryGetValue("CurrentActiveCount", out var countObj))
        {
            var count = Convert.ToInt32(countObj);
            for (int i = 0; i < count; i++)
            {
                specialist.IncrementActiveTickets();
            }
        }

        return specialist;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var users = await GetAllAsync();
        return users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<List<User>> GetByUserTypeAsync(UserType userType)
    {
        var users = await GetAllAsync();
        return users.Where(u => u.GetUserType() == userType).ToList();
    }

    public async Task<List<SupportSpecialist>> GetSpecialistsByTeamAsync(string teamId)
    {
        var users = await GetAllAsync();
        return users
            .OfType<SupportSpecialist>()
            .Where(s => s.TeamId == teamId)
            .ToList();
    }
}
