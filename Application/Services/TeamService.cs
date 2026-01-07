using TicketingSystem.Application.Mappers;
using TicketingSystem.Domain.Aggregates.Team;
using TicketingSystem.Domain.Aggregates.User;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Exceptions;
using TicketingSystem.Infrastructure.Persistence;

namespace TicketingSystem.Application.Services;

public class TeamService
{
    private readonly TeamRepository _teamRepository;
    private readonly UserRepository _userRepository;
    private readonly TeamMapper _teamMapper;

    public TeamService(TeamRepository teamRepository, UserRepository userRepository, TeamMapper teamMapper)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _teamMapper = teamMapper;
    }

    public async Task<Team> CreateTeamAsync(string id, string name, TicketCategory specialization, int maxTickets = 50)
    {
        var team = Team.Create(id, name, specialization, maxTickets);
        await _teamRepository.SaveAsync(team);
        return team;
    }

    public async Task<Team> GetTeamByIdAsync(string teamId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team is null)
        {
            throw new NotFoundException("TEAM_NOT_FOUND", teamId);
        }
        return team;
    }

    public async Task AddSpecialistToTeamAsync(string teamId, string specialistId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team is null)
        {
            throw new NotFoundException("TEAM_NOT_FOUND", teamId);
        }

        var specialist = await _userRepository.GetByIdAsync(specialistId);
        if (specialist is not SupportSpecialist)
        {
            throw new ValidationException("USER_NOT_SPECIALIST", specialistId);
        }

        team.AddSpecialist(specialistId);
        await _teamRepository.SaveAsync(team);
    }

    public async Task RemoveSpecialistFromTeamAsync(string teamId, string specialistId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team is null)
        {
            throw new NotFoundException("TEAM_NOT_FOUND", teamId);
        }

        team.RemoveSpecialist(specialistId);
        await _teamRepository.SaveAsync(team);
    }
}
