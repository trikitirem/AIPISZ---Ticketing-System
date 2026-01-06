using TicketingSystem.Application.DTOs;
using TicketingSystem.Domain.Aggregates.Team;

namespace TicketingSystem.Application.Mappers;

/// <summary>
/// Mapper do konwersji Team na DTOs.
/// </summary>
public class TeamMapper
{
    public TeamDTO Map(Team team)
    {
        return new TeamDTO
        {
            Id = team.Id,
            Name = team.Name,
            Specialization = team.Specialization.ToString(),
            SpecialistCount = team.GetSpecialistCount()
        };
    }

    public List<TeamDTO> MapList(IEnumerable<Team> teams)
    {
        return teams.Select(Map).ToList();
    }
}
