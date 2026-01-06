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
            throw new ConflictException("USER_EMAIL_ALREADY_EXISTS", email);
        }

        User user = userType switch
        {
            UserType.WORKER => Worker.Create(id, email, firstName, lastName, accountStatus),
            UserType.SPECIALIST => SupportSpecialist.Create(id, email, firstName, lastName, accountStatus),
            UserType.ADMINISTRATOR => Administrator.Create(id, email, firstName, lastName, accountStatus),
            _ => throw new ValidationException("INVALID_USER_TYPE", userType.ToString())
        };

        await _userRepository.SaveAsync(user);
        return user;
    }

    public async Task<User> GetUserByIdAsync(string userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException("USER_NOT_FOUND", userId);
        }
        return user;
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            throw new NotFoundException("USER_NOT_FOUND", email);
        }
        return user;
    }

    public async Task<User> AuthenticateAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            throw new UnauthorizedException("INVALID_EMAIL", email);
        }

        if (!user.IsActive())
        {
            throw new ForbiddenException("USER_ACCOUNT_NOT_ACTIVE", email);
        }

        return user;
    }
}
