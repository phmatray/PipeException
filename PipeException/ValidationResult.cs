namespace PipeException;

public readonly struct ValidationResult<T>
{
    private readonly T _value;
    private readonly Func<T, bool> _predicate;
    private readonly string? _message;
    private readonly string? _predicateExpression;

    internal ValidationResult(T value, Func<T, bool> predicate, string? message = null, string? predicateExpression = null)
    {
        _value = value;
        _predicate = predicate;
        _message = message;
        _predicateExpression = predicateExpression;
    }

    /// <summary>
    /// Allows chaining validation with another predicate.
    /// </summary>
    public static ValidationResult<T> operator |(ValidationResult<T> result, Func<T, bool> predicate)
    {
        // First validate the previous condition
        T value = result;
        // Then return a new ValidationResult for the next predicate
        return new ValidationResult<T>(value, predicate, null, null);
    }

    /// <summary>
    /// Allows chaining validation with another predicate and custom message.
    /// </summary>
    public static ValidationResult<T> operator |(ValidationResult<T> result, (Func<T, bool> predicate, string message) validation)
    {
        // First validate the previous condition
        T value = result;
        // Then return a new ValidationResult for the next predicate
        return new ValidationResult<T>(value, validation.predicate, validation.message, null);
    }

    public T OrThrow<TException>() where TException : Exception, new()
    {
        if (!_predicate(_value))
        {
            var exception = new TException();
            throw exception;
        }
        return _value;
    }

    public T OrThrow<TException>(Func<string, TException> exceptionFactory) where TException : Exception
    {
        if (!_predicate(_value))
        {
            var errorMessage = _message ?? $"Condition not met: {_predicateExpression}";
            throw exceptionFactory(errorMessage);
        }
        return _value;
    }

    public T OrThrow(Func<Exception> exceptionFactory)
    {
        if (!_predicate(_value))
        {
            throw exceptionFactory();
        }
        return _value;
    }

    public T OrThrowNull(string? paramName = null)
    {
        if (!_predicate(_value))
        {
            var errorMessage = _message ?? $"Condition not met: {_predicateExpression}";
            throw new ArgumentNullException(paramName, errorMessage);
        }
        return _value;
    }

    public T OrThrowInvalidOperation()
    {
        if (!_predicate(_value))
        {
            var errorMessage = _message ?? $"Condition not met: {_predicateExpression}";
            throw new InvalidOperationException(errorMessage);
        }
        return _value;
    }

    public static implicit operator T(ValidationResult<T> result)
    {
        if (!result._predicate(result._value))
        {
            var errorMessage = result._message ?? $"Condition not met: {result._predicateExpression}";
            throw new ArgumentException(errorMessage);
        }
        return result._value;
    }
}
