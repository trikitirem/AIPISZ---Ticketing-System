using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Application.Mappers;
using TicketingSystem.Application.Services;
using TicketingSystem.Domain.Aggregates.User;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Infrastructure.Persistence;

namespace TicketingSystem.Presentation.Controllers;

/// <summary>
/// Kontroler do zarządzania zespołami.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly TeamService _teamService;
    private readonly TeamMapper _teamMapper;
    private readonly UserRepository _userRepository;
    private readonly UserMapper _userMapper;

    public TeamsController(TeamService teamService, TeamMapper teamMapper, UserRepository userRepository, UserMapper userMapper)
    {
        _teamService = teamService;
        _teamMapper = teamMapper;
        _userRepository = userRepository;
        _userMapper = userMapper;
    }

    [HttpPost]
    public async Task<ActionResult<TeamDTO>> CreateTeam([FromBody] CreateTeamRequest request)
    {
        var specialization = Enum.Parse<TicketCategory>(request.Specialization, true);
        var teamId = Guid.NewGuid().ToString();

        var team = await _teamService.CreateTeamAsync(teamId, request.Name, specialization, request.MaxTickets);
        var dto = _teamMapper.Map(team);

        return CreatedAtAction(nameof(GetTeam), new { id = team.Id }, dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDTO>> GetTeam(string id)
    {
        var team = await _teamService.GetTeamByIdAsync(id);
        var dto = _teamMapper.Map(team);

        return Ok(dto);
    }

    [HttpGet("{id}/members")]
    public async Task<ActionResult<List<SupportSpecialistDTO>>> GetTeamMembers(string id)
    {
        var specialists = await _userRepository.GetSpecialistsByTeamAsync(id);
        var dtos = specialists.Select(s => _userMapper.MapSpecialist(s)).ToList();

        return Ok(dtos);
    }

    [HttpPost("{id}/specialists")]
    public async Task<IActionResult> AddSpecialist(string id, [FromBody] AddSpecialistToTeamRequest request)
    {
        await _teamService.AddSpecialistToTeamAsync(id, request.SpecialistId);

        return NoContent();
    }
}
