namespace TicketingSystem.Domain.Exceptions;

/// <summary>
/// Wyjątek rzucany gdy występuje konflikt stanu (np. nieprawidłowa zmiana statusu).
/// </summary>
public class ConflictException : DomainException
{
    public ConflictException(string message, string details) 
        : base(409, message, details)
    {
    }

    public ConflictException(string message, string details, Exception innerException) 
        : base(409, message, details, innerException)
    {
    }
}
