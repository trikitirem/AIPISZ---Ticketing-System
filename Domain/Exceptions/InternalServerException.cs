namespace TicketingSystem.Domain.Exceptions;

/// <summary>
/// Wyjątek rzucany gdy wystąpił nieoczekiwany błąd wewnętrzny.
/// </summary>
public class InternalServerException : DomainException
{
    public InternalServerException(string message, string details) 
        : base(500, message, details)
    {
    }

    public InternalServerException(string message, string details, Exception innerException) 
        : base(500, message, details, innerException)
    {
    }
}
