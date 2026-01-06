using TicketingSystem.Domain.ValueObjects;

namespace TicketingSystem.Domain.Policies;

/// <summary>
/// Abstrakcyjna klasa bazowa dla wszystkich policies w systemie.
/// Policies zawierają logikę biznesową i są synchroniczne (nie mają dependencies na DB).
/// </summary>
public abstract class Policy
{
    /// <summary>
    /// Helper method zwracający Result<bool> z sukcesem.
    /// </summary>
    protected static Result<bool> Success()
    {
        return Result<bool>.CreateSuccess(true);
    }

    /// <summary>
    /// Helper method zwracający Result<bool> z błędem.
    /// </summary>
    /// <param name="message">Komunikat błędu</param>
    protected static Result<bool> Failure(string message)
    {
        return Result<bool>.CreateFailure(message);
    }
}
