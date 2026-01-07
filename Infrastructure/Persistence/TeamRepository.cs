using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using TicketingSystem.Domain.Aggregates.Team;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Infrastructure.Persistence;

/// <summary>
/// Repozytorium dla agregatu Team oparte na plikach JSON.
/// </summary>
public class TeamRepository : FileBasedRepository<Team>
{
    public TeamRepository(ILogger<TeamRepository> logger)
        : base("Data/teams.json", logger)
    {
    }

    protected override Team? FromPrimitive(Dictionary<string, object> data)
    {
        try
        {
            var id = data["Id"].ToString()!;
            var name = data["Name"].ToString()!;
            var specialization = Enum.Parse<TicketCategory>(data["Specialization"].ToString()!);
            var maxTickets = Convert.ToInt32(data["MaxTickets"]);

            var team = Team.Create(id, name, specialization, maxTickets);

            if (data.TryGetValue("SpecialistIds", out var specialistIdsObj) && specialistIdsObj is not null)
            {
                var specialistIds = JsonSerializer.Deserialize<List<string>>(JsonSerializer.Serialize(specialistIdsObj));
                if (specialistIds is not null)
                {
                    foreach (var specialistId in specialistIds)
                    {
                        try
                        {
                            team.AddSpecialist(specialistId);
                        }
                        catch
                        {
                            // Ignore duplicates or invalid IDs
                        }
                    }
                }
            }

            return team;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deserializing Team from primitive data");
            return null;
        }
    }

    public async Task<List<Team>> GetBySpecializationAsync(TicketCategory specialization)
    {
        var teams = await GetAllAsync();
        return teams.Where(t => t.Specialization == specialization).ToList();
    }
}
