namespace TicketingSystem.Domain.Exceptions;

/// <summary>
/// Wyjątek rzucany gdy użytkownik nie ma uprawnień do wykonania operacji.
/// </summary>
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message, string details) 
        : base(403, message, details)
    {
    }

    public ForbiddenException(string message, string details, Exception innerException) 
        : base(403, message, details, innerException)
    {
    }
}
