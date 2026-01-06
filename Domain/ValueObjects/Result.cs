using TicketingSystem.Domain.Base;
using DomainExceptions = TicketingSystem.Domain.Exceptions;

namespace TicketingSystem.Domain.ValueObjects;

/// <summary>
/// Value Object reprezentujący wynik operacji (sukces lub błąd).
/// </summary>
/// <typeparam name="T">Typ wartości w przypadku sukcesu</typeparam>
public class Result<T> : ValueObject
{
    public bool IsSuccess { get; private set; }

    public T? Value { get; private set; }

    public string Error { get; private set; }

    private Result(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> CreateSuccess(T value)
    {
        return new Result<T>(true, value, string.Empty);
    }

    public static Result<T> CreateFailure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new DomainExceptions.ValidationException("RESULT_DATA_VALIDATION_ERROR", "Error message cannot be null or empty");
        }

        return new Result<T>(false, default, error);
    }

    public override bool Equals(ValueObject? other)
    {
        if (other is not Result<T> otherResult)
            return false;

        if (IsSuccess != otherResult.IsSuccess)
            return false;

        if (IsSuccess)
        {
            if (Value is null && otherResult.Value is null)
                return true;

            if (Value is null || otherResult.Value is null)
                return false;

            return Value.Equals(otherResult.Value);
        }

        return Error == otherResult.Error;
    }

    public override int GetHashCode()
    {
        var hash = IsSuccess.GetHashCode();
        
        if (IsSuccess && Value is not null)
        {
            hash = HashCode.Combine(hash, Value.GetHashCode());
        }
        else if (!IsSuccess)
        {
            hash = HashCode.Combine(hash, Error.GetHashCode());
        }

        return hash;
    }
}
