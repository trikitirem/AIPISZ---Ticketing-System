using TicketingSystem.Domain.Base;

namespace TicketingSystem.Infrastructure.Persistence;

/// <summary>
/// Interfejs repozytorium dla agregatów.
/// </summary>
/// <typeparam name="T">Typ agregatu</typeparam>
public interface IRepository<T> where T : AggregateRoot
{
    /// <summary>
    /// Pobiera agregat po identyfikatorze.
    /// </summary>
    Task<T?> GetByIdAsync(string id);

    /// <summary>
    /// Zapisuje agregat.
    /// </summary>
    Task SaveAsync(T aggregate);

    /// <summary>
    /// Pobiera wszystkie agregaty.
    /// </summary>
    Task<List<T>> GetAllAsync();

    /// <summary>
    /// Usuwa agregat po identyfikatorze.
    /// </summary>
    Task DeleteAsync(string id);
}
