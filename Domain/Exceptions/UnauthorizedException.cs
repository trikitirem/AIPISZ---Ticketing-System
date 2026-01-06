namespace TicketingSystem.Domain.Exceptions;

/// <summary>
/// Wyjątek rzucany gdy użytkownik nie jest uwierzytelniony.
/// </summary>
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message, string details) 
        : base(401, message, details)
    {
    }

    public UnauthorizedException(string message, string details, Exception innerException) 
        : base(401, message, details, innerException)
    {
    }
}
