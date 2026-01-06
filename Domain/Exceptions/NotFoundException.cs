namespace TicketingSystem.Domain.Exceptions;

/// <summary>
/// Wyjątek rzucany gdy zasób nie został znaleziony.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string message, string details) 
        : base(404, message, details)
    {
    }

    public NotFoundException(string message, string details, Exception innerException) 
        : base(404, message, details, innerException)
    {
    }
}
