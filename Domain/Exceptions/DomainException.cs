namespace TicketingSystem.Domain.Exceptions;

/// <summary>
/// Klasa bazowa dla wszystkich wyjątków domenowych.
/// </summary>
public abstract class DomainException : Exception
{
    public int Code { get; }
    public string Details { get; }

    protected DomainException(int code, string message, string details) 
        : base(message)
    {
        Code = code;
        Details = details;
    }

    protected DomainException(int code, string message, string details, Exception innerException) 
        : base(message, innerException)
    {
        Code = code;
        Details = details;
    }
}
