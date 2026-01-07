using System.Collections.Generic;

namespace TicketingSystem.Domain.Base;

/// <summary>
/// Klasa bazowa dla wszystkich encji w domenie.
/// Encje są identyfikowane po ID i mają możliwość serializacji/deserializacji.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Unikalny identyfikator encji.
    /// </summary>
    public string Id { get; protected set; } = string.Empty;

    /// <summary>
    /// Prywatny konstruktor - wymusza użycie metody Create().
    /// </summary>
    protected Entity()
    {
    }

    /// <summary>
    /// Factory method do tworzenia encji.
    /// </summary>
    /// <typeparam name="T">Typ encji do utworzenia</typeparam>
    /// <param name="id">Identyfikator encji</param>
    /// <returns>Nowa instancja encji typu T</returns>
    public static T Create<T>(string id) where T : Entity, new()
    {
        var entity = new T();
        entity.Id = id;
        return entity;
    }

    /// <summary>
    /// Konwertuje encję do słownika (dla serializacji).
    /// </summary>
    /// <returns>Słownik z właściwościami encji</returns>
    public abstract Dictionary<string, object> ToPrimitive();

    /// <summary>
    /// Tworzy encję ze słownika (dla deserializacji).
    /// </summary>
    /// <typeparam name="T">Typ encji do utworzenia</typeparam>
    /// <param name="data">Słownik z danymi</param>
    /// <returns>Nowa instancja encji typu T</returns>
    public static T FromPrimitive<T>(Dictionary<string, object> data) where T : Entity, new()
    {
        var entity = new T();
        // Implementacja deserializacji powinna być przesłonięta w klasach pochodnych
        // lub używać refleksji do ustawienia właściwości
        return entity;
    }

    /// <summary>
    /// Porównuje dwie encje po ID.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is not Entity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (Id is null || other.Id is null)
            return false;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Zwraca hash code na podstawie ID.
    /// </summary>
    public override int GetHashCode()
    {
        return Id?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// Operator równości dla encji.
    /// </summary>
    public static bool operator ==(Entity? left, Entity? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Operator nierówności dla encji - przeciwieństwo operatora ==.
    /// </summary>
    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}
