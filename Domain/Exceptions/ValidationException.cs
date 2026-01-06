namespace TicketingSystem.Domain.Exceptions;

/// <summary>
/// Wyjątek rzucany gdy dane wejściowe są nieprawidłowe.
/// </summary>
public class ValidationException : DomainException
{
    public ValidationException(string message, string details) 
        : base(400, message, details)
    {
    }

    public ValidationException(string message, string details, Exception innerException) 
        : base(400, message, details, innerException)
    {
    }
}
