namespace PipeException;

public static class Validate
{
    // String validators
    public static Func<string?, bool> NotNullOrEmpty => s => !string.IsNullOrEmpty(s);
    public static Func<string?, bool> NotNullOrWhiteSpace => s => !string.IsNullOrWhiteSpace(s);

    // Generic not null for reference types
    public static Func<T?, bool> NotNull<T>() where T : class => x => x is not null;

    // Int validators
    public static Func<int, bool> Positive => x => x > 0;
    public static Func<int, bool> NonNegative => x => x >= 0;
    public static Func<int, bool> Negative => x => x < 0;

    // Long validators
    public static Func<long, bool> PositiveLong => x => x > 0;
    public static Func<long, bool> NonNegativeLong => x => x >= 0;
    public static Func<long, bool> NegativeLong => x => x < 0;

    // Double validators
    public static Func<double, bool> PositiveDouble => x => x > 0;
    public static Func<double, bool> NonNegativeDouble => x => x >= 0;
    public static Func<double, bool> NegativeDouble => x => x < 0;

    // Float validators
    public static Func<float, bool> PositiveFloat => x => x > 0;
    public static Func<float, bool> NonNegativeFloat => x => x >= 0;
    public static Func<float, bool> NegativeFloat => x => x < 0;

    // Decimal validators
    public static Func<decimal, bool> PositiveDecimal => x => x > 0;
    public static Func<decimal, bool> NonNegativeDecimal => x => x >= 0;
    public static Func<decimal, bool> NegativeDecimal => x => x < 0;

    // Range validators
    public static Func<int, bool> InRange(int min, int max) => x => x >= min && x <= max;
    public static Func<long, bool> InRange(long min, long max) => x => x >= min && x <= max;
    public static Func<double, bool> InRange(double min, double max) => x => x >= min && x <= max;
    public static Func<float, bool> InRange(float min, float max) => x => x >= min && x <= max;
    public static Func<decimal, bool> InRange(decimal min, decimal max) => x => x >= min && x <= max;

    // Collection validators
    public static Func<IEnumerable<T>?, bool> NotEmpty<T>() => c => c?.Any() == true;
    public static Func<ICollection<T>?, bool> HasCount<T>(int count) => c => c?.Count == count;
    public static Func<ICollection<T>?, bool> HasMinCount<T>(int minCount) => c => c?.Count >= minCount;
    public static Func<ICollection<T>?, bool> HasMaxCount<T>(int maxCount) => c => c?.Count <= maxCount;
}
