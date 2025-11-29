namespace PipeException;

public static class NullableExtensions
{
    extension<T>(T? source) where T : struct
    {
        public T EnsureNotNull(string? message = null)
        {
            return source ?? throw new ArgumentNullException(null, message ?? "Value cannot be null");
        }

        public T EnsureNotNull(string paramName, string? message)
        {
            return source ?? throw new ArgumentNullException(paramName, message ?? "Value cannot be null");
        }
    }

    extension<T>(T? source) where T : class
    {
        public T EnsureNotNull(string? message = null)
        {
            return source ?? throw new ArgumentNullException(null, message ?? "Value cannot be null");
        }

        public T EnsureNotNull(string paramName, string? message)
        {
            return source ?? throw new ArgumentNullException(paramName, message ?? "Value cannot be null");
        }
    }
}
