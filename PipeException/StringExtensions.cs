namespace PipeException;

public static class StringExtensions
{
    extension(string? source)
    {
        public string EnsureNotNullOrEmpty(string? message = null)
        {
            return string.IsNullOrEmpty(source)
                ? throw new ArgumentException(message ?? "String cannot be null or empty")
                : source;
        }

        public string EnsureNotNullOrWhiteSpace(string? message = null)
        {
            return string.IsNullOrWhiteSpace(source)
                ? throw new ArgumentException(message ?? "String cannot be null or whitespace")
                : source;
        }

        public string EnsureMinLength(int minLength, string? message = null)
        {
            return source is null || source.Length < minLength
                ? throw new ArgumentException(message ?? $"String must be at least {minLength} characters")
                : source;
        }

        public string EnsureMaxLength(int maxLength, string? message = null)
        {
            return source is not null && source.Length > maxLength
                ? throw new ArgumentException(message ?? $"String must be at most {maxLength} characters")
                : source ?? string.Empty;
        }
    }
}
