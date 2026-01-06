using TicketingSystem.Application.Mappers;
using TicketingSystem.Domain.Aggregates.User;
using TicketingSystem.Domain.Enums;
using TicketingSystem.Domain.Exceptions;
using TicketingSystem.Infrastructure.Persistence;

namespace TicketingSystem.Application.Services;

/// <summary>
/// Service do zarządzania użytkownikami.
/// </summary>
public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly UserMapper _userMapper;

    public UserService(UserRepository userRepository, UserMapper userMapper)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
    }

    public async Task<User> RegisterUserAsync(string id, string email, string firstName, string lastName, UserType userType, AccountStatusEnum accountStatus = AccountStatusEnum.ACTIVE)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email);
        if (existingUser is not null)
        {
            throw new ConflictException("User with this email already exists", email);
        }

        User user = userType switch
        {
            UserType.WORKER => Worker.Create(id, email, firstName, lastName, accountStatus),
            UserType.SPECIALIST => SupportSpecialist.Create(id, email, firstName, lastName, accountStatus),
            UserType.ADMINISTRATOR => Administrator.Create(id, email, firstName, lastName, accountStatus),
            _ => throw new ValidationException("Invalid user type", userType.ToString())
        };

        await _userRepository.SaveAsync(user);
        return user;
    }

    public async Task<User> GetUserByIdAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("User not found", userId);
        }
        return user;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            throw new NotFoundException("User not found", email);
        }
        return user;
    }

    public async Task<User> AuthenticateAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            throw new UnauthorizedException("Invalid email", email);
        }

        if (!user.IsActive())
        {
            throw new ForbiddenException("User account is not active", email);
        }

        return user;
    }
}
