namespace TicketingSystem.Domain.Base;

/// <summary>
/// Klasa bazowa dla wszystkich agregatów (Aggregate Roots) w domenie.
/// Agregaty zarządzają kolekcją encji i value objects, zapewniając spójność domeny.
/// </summary>
public abstract class AggregateRoot : Entity
{
    /// <summary>
    /// Prywatny konstruktor - wymusza użycie metody Create() z klasy bazowej Entity.
    /// </summary>
    protected AggregateRoot()
    {
    }
}
