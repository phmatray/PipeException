namespace PipeException;

using System.Runtime.CompilerServices;

public static class PipeExceptionOperations
{
    extension<T>(T source)
    {
        /// <summary>
        /// Pipe operator for validation. Returns ValidationResult for deferred exception selection.
        /// Implicitly converts to T (throwing ArgumentException if validation fails).
        /// </summary>
        public static ValidationResult<T> operator |(T left, Func<T, bool> predicate)
        {
            return new ValidationResult<T>(left, predicate, null, null);
        }

        /// <summary>
        /// Pipe operator with custom message. Returns ValidationResult for deferred exception selection.
        /// </summary>
        public static ValidationResult<T> operator |(T left, (Func<T, bool> predicate, string message) validation)
        {
            return new ValidationResult<T>(left, validation.predicate, validation.message, null);
        }

        /// <summary>
        /// Validates that the condition is met, throwing ArgumentException if not.
        /// Returns the original value for chaining.
        /// </summary>
        public T Ensure(
            Func<T, bool> predicate,
            string? message = null,
            [CallerArgumentExpression(nameof(predicate))] string? predicateExpression = null)
        {
            if (!predicate(source))
            {
                var errorMessage = message ?? $"Condition not met: {predicateExpression}";
                throw new ArgumentException(errorMessage);
            }

            return source;
        }

        /// <summary>
        /// Validates that the condition is met, throwing the specified exception if not.
        /// </summary>
        public T Ensure<TException>(
            Func<T, bool> predicate,
            Func<TException> exceptionFactory) where TException : Exception
        {
            return !predicate(source)
                ? throw exceptionFactory()
                : source;
        }
    }
}
