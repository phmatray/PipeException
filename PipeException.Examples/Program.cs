using PipeException;

Console.WriteLine("=== PipeException Library Examples ===\n");

// ============================================
// 1. BASIC PIPE OPERATOR
// ============================================
Console.WriteLine("1. Basic Pipe Operator");
Console.WriteLine("-".PadRight(40, '-'));

int age = 25;
int validatedAge = age | (x => x >= 0);
Console.WriteLine($"   age | (x => x >= 0) = {validatedAge}");

int score = 85;
int validatedScore = score | (x => x >= 0 && x <= 100, "Score must be between 0 and 100");
Console.WriteLine($"   score | (x => x >= 0 && x <= 100, \"...\") = {validatedScore}");

Console.WriteLine();

// ============================================
// 2. CHAINED VALIDATIONS
// ============================================
Console.WriteLine("2. Chained Validations");
Console.WriteLine("-".PadRight(40, '-'));

int value = 50;
int chainedResult = value
    | (x => x > 0)
    | (x => x < 100)
    | (x => x % 2 == 0);
Console.WriteLine($"   50 | (x > 0) | (x < 100) | (x % 2 == 0) = {chainedResult}");

Console.WriteLine();

// ============================================
// 3. DIFFERENT EXCEPTION TYPES
// ============================================
Console.WriteLine("3. Different Exception Types");
Console.WriteLine("-".PadRight(40, '-'));

int positiveValue = 10;

// OrThrowInvalidOperation
var result1 = (positiveValue | (x => x > 0)).OrThrowInvalidOperation();
Console.WriteLine($"   OrThrowInvalidOperation(): {result1}");

// OrThrowNull
var result2 = (positiveValue | (x => x > 0)).OrThrowNull("value");
Console.WriteLine($"   OrThrowNull(\"value\"): {result2}");

// OrThrow with custom factory
var result3 = (positiveValue | (x => x > 0)).OrThrow(() => new InvalidOperationException("Custom error"));
Console.WriteLine($"   OrThrow(() => new InvalidOperationException(...)): {result3}");

// OrThrow with message factory
var result4 = (positiveValue | (x => x > 0)).OrThrow(msg => new ArgumentException(msg));
Console.WriteLine($"   OrThrow(msg => new ArgumentException(msg)): {result4}");

Console.WriteLine();

// ============================================
// 4. STATIC VALIDATORS (Validate class)
// ============================================
Console.WriteLine("4. Static Validators (Validate class)");
Console.WriteLine("-".PadRight(40, '-'));

// Numeric validators
int num = 42;
int positive = num | Validate.Positive;
Console.WriteLine($"   {num} | Validate.Positive = {positive}");

int zero = 0;
int nonNegative = zero | Validate.NonNegative;
Console.WriteLine($"   {zero} | Validate.NonNegative = {nonNegative}");

int inRange = num | Validate.InRange(0, 100);
Console.WriteLine($"   {num} | Validate.InRange(0, 100) = {inRange}");

// Long validators
long bigNum = 1_000_000L;
long positiveLong = bigNum | Validate.PositiveLong;
Console.WriteLine($"   {bigNum}L | Validate.PositiveLong = {positiveLong}");

// Double validators
double pi = 3.14159;
double positiveDouble = pi | Validate.PositiveDouble;
Console.WriteLine($"   {pi} | Validate.PositiveDouble = {positiveDouble}");

// Decimal validators
decimal price = 99.99m;
decimal positiveDecimal = price | Validate.PositiveDecimal;
Console.WriteLine($"   {price}m | Validate.PositiveDecimal = {positiveDecimal}");

// String validators
string name = "Alice";
string notEmpty = name | Validate.NotNullOrEmpty;
Console.WriteLine($"   \"{name}\" | Validate.NotNullOrEmpty = \"{notEmpty}\"");

// Collection validators - use EnsureNotEmpty extension instead
var numbers = new List<int> { 1, 2, 3 };
var notEmptyList = numbers.EnsureNotEmpty();
Console.WriteLine($"   [1,2,3].EnsureNotEmpty() = [{string.Join(",", notEmptyList)}]");

Console.WriteLine();

// ============================================
// 5. STRING EXTENSIONS
// ============================================
Console.WriteLine("5. String Extensions");
Console.WriteLine("-".PadRight(40, '-'));

string username = "john_doe";

var ensured1 = username.EnsureNotNullOrEmpty();
Console.WriteLine($"   \"{username}\".EnsureNotNullOrEmpty() = \"{ensured1}\"");

var ensured2 = username.EnsureNotNullOrWhiteSpace();
Console.WriteLine($"   \"{username}\".EnsureNotNullOrWhiteSpace() = \"{ensured2}\"");

var ensured3 = username.EnsureMinLength(3);
Console.WriteLine($"   \"{username}\".EnsureMinLength(3) = \"{ensured3}\"");

var ensured4 = username.EnsureMaxLength(50);
Console.WriteLine($"   \"{username}\".EnsureMaxLength(50) = \"{ensured4}\"");

Console.WriteLine();

// ============================================
// 6. NULLABLE EXTENSIONS
// ============================================
Console.WriteLine("6. Nullable Extensions");
Console.WriteLine("-".PadRight(40, '-'));

// Nullable value type
int? nullableInt = 42;
int unwrappedInt = nullableInt.EnsureNotNull();
Console.WriteLine($"   (int?)42.EnsureNotNull() = {unwrappedInt}");

int? anotherNullable = 100;
int unwrapped2 = anotherNullable.EnsureNotNull("myParam", "Value is required");
Console.WriteLine($"   (int?)100.EnsureNotNull(\"myParam\", \"...\") = {unwrapped2}");

// Nullable reference type
string? nullableString = "Hello";
string unwrappedString = nullableString.EnsureNotNull();
Console.WriteLine($"   (string?)\"Hello\".EnsureNotNull() = \"{unwrappedString}\"");

Console.WriteLine();

// ============================================
// 7. COLLECTION EXTENSIONS
// ============================================
Console.WriteLine("7. Collection Extensions");
Console.WriteLine("-".PadRight(40, '-'));

var items = new List<int> { 1, 2, 3, 4, 5 };

var notEmptyCollection = items.EnsureNotEmpty();
Console.WriteLine($"   [1,2,3,4,5].EnsureNotEmpty() = [{string.Join(",", notEmptyCollection)}]");

var notNullCollection = items.EnsureNotNull();
Console.WriteLine($"   [1,2,3,4,5].EnsureNotNull() = [{string.Join(",", notNullCollection)}]");

var exactCount = items.EnsureCount(5);
Console.WriteLine($"   [1,2,3,4,5].EnsureCount(5) = [{string.Join(",", exactCount)}]");

var minCount = items.EnsureMinCount(3);
Console.WriteLine($"   [1,2,3,4,5].EnsureMinCount(3) = [{string.Join(",", minCount)}]");

var maxCount = items.EnsureMaxCount(10);
Console.WriteLine($"   [1,2,3,4,5].EnsureMaxCount(10) = [{string.Join(",", maxCount)}]");

Console.WriteLine();

// ============================================
// 8. ERROR HANDLING EXAMPLES
// ============================================
Console.WriteLine("8. Error Handling Examples");
Console.WriteLine("-".PadRight(40, '-'));

// Demonstrate exception throwing
try
{
    int invalid = -5;
    int _ = invalid | (x => x >= 0);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"   Caught ArgumentException: {ex.Message}");
}

try
{
    int invalid = -5;
    var _ = (invalid | (x => x >= 0)).OrThrowInvalidOperation();
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"   Caught InvalidOperationException: {ex.Message}");
}

try
{
    string? nullString = null;
    var _ = nullString.EnsureNotNull();
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"   Caught ArgumentNullException: {ex.Message}");
}

try
{
    var emptyList = new List<int>();
    var _ = emptyList.EnsureNotEmpty("List cannot be empty");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"   Caught ArgumentException: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== All examples completed successfully! ===");
