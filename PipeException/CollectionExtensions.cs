namespace PipeException;

public static class CollectionExtensions
{
    extension<T>(IEnumerable<T>? source)
    {
        public IEnumerable<T> EnsureNotEmpty(string? message = null)
        {
            return source is null || !source.Any()
                ? throw new ArgumentException(message ?? "Collection cannot be null or empty")
                : source;
        }

        public IEnumerable<T> EnsureNotNull(string? message = null)
        {
            return source ?? throw new ArgumentNullException(null, message ?? "Collection cannot be null");
        }
    }

    extension<T>(ICollection<T>? source)
    {
        public ICollection<T> EnsureCount(int count, string? message = null)
        {
            return source?.Count != count
                ? throw new ArgumentException(message ?? $"Collection must have exactly {count} items")
                : source;
        }

        public ICollection<T> EnsureMinCount(int minCount, string? message = null)
        {
            return source is null || source.Count < minCount
                ? throw new ArgumentException(message ?? $"Collection must have at least {minCount} items")
                : source;
        }

        public ICollection<T> EnsureMaxCount(int maxCount, string? message = null)
        {
            return source is not null && source.Count > maxCount
                ? throw new ArgumentException(message ?? $"Collection must have at most {maxCount} items")
                : source ?? throw new ArgumentNullException();
        }
    }
}
