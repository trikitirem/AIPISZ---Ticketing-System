using System.Collections.Generic;

namespace TicketingSystem.Domain.Base;

/// <summary>
/// Klasa bazowa dla wszystkich agregatów (Aggregate Roots) w domenie.
/// Agregaty zarządzają kolekcją encji i value objects, zapewniając spójność domeny.
/// </summary>
/// <typeparam name="TId">Typ identyfikatora agregatu (np. string, Guid, int)</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<object> _uncommittedChanges = new();

    /// <summary>
    /// Prywatny konstruktor - wymusza użycie metody Create() z klasy bazowej Entity.
    /// </summary>
    protected AggregateRoot()
    {
    }

    /// <summary>
    /// Zwraca listę niezatwierdzonych zmian (zdarzeń domenowych).
    /// </summary>
    public IReadOnlyList<object> GetUncommittedChanges()
    {
        return _uncommittedChanges.AsReadOnly();
    }

    /// <summary>
    /// Czyści listę niezatwierdzonych zmian (po zapisaniu do repozytorium).
    /// </summary>
    public void ClearUncommittedChanges()
    {
        _uncommittedChanges.Clear();
    }

    /// <summary>
    /// Dodaje zdarzenie domenowe do listy niezatwierdzonych zmian.
    /// </summary>
    /// <param name="event">Zdarzenie domenowe</param>
    protected void RaiseEvent(object @event)
    {
        if (@event is null)
            return;

        _uncommittedChanges.Add(@event);
    }
}
