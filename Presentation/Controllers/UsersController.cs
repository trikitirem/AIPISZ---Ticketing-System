using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Application.DTOs;
using TicketingSystem.Application.Mappers;
using TicketingSystem.Application.Services;
using TicketingSystem.Domain.Enums;

namespace TicketingSystem.Presentation.Controllers;

/// <summary>
/// Kontroler do zarządzania użytkownikami.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly UserMapper _userMapper;

    public UsersController(UserService userService, UserMapper userMapper)
    {
        _userService = userService;
        _userMapper = userMapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> RegisterUser([FromBody] RegisterUserRequest request)
    {
        var userType = Enum.Parse<UserType>(request.UserType, true);
        var userId = Guid.NewGuid().ToString();

        var user = await _userService.RegisterUserAsync(userId, request.Email, request.FirstName, request.LastName, userType);
        var dto = _userMapper.Map(user);

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, dto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUser(string id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        var dto = _userMapper.Map(user);

        return Ok(dto);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.AuthenticateAsync(request.Email);
        var dto = _userMapper.Map(user);

        return Ok(dto);
    }
}
