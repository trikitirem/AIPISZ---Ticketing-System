namespace TicketingSystem.Domain.Base;

/// <summary>
/// Klasa bazowa dla wszystkich Value Objects.
/// Value Objects są porównywane po wartościach, nie po referencji.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Porównuje dwa Value Objects po ich wartościach.
    /// </summary>
    public abstract bool Equals(ValueObject? other);

    /// <summary>
    /// Przesłonięta metoda Equals dla porównania z dowolnym obiektem.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;

        if (obj is not ValueObject other)
            return false;

        return Equals(other);
    }

    /// <summary>
    /// Zwraca hash code na podstawie wartości wszystkich właściwości.
    /// </summary>
    public abstract override int GetHashCode();

    /// <summary>
    /// Operator równości dla Value Objects.
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (ReferenceEquals(left, right))
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Operator nierówności dla Value Objects.
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
