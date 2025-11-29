# PipeException

[![NuGet](https://img.shields.io/nuget/v/PipeException.svg)](https://www.nuget.org/packages/PipeException/)
[![CI](https://github.com/phmatray/PipeException/actions/workflows/ci.yml/badge.svg)](https://github.com/phmatray/PipeException/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A fluent validation and guard clause library for .NET using pipe operators and C# extensions.

## Features

- **Pipe Operator Validation**: Use the `|` operator for inline validation
- **Fluent Chaining**: Chain multiple validations together seamlessly
- **Custom Exception Types**: Choose which exception to throw after validation
- **Built-in Validators**: Common validators for strings, collections, nullables, and numeric types
- **Modern C# Syntax**: Leverages C# extension methods for clean, readable code

## Requirements

- .NET 10.0 or later

## Installation

```bash
dotnet add package PipeException
```

## Quick Start

### Basic Validation

```csharp
using PipeException;

// Throws ArgumentException if condition is false
int positiveNumber = value | (x => x > 0);

// With custom error message
int rangedValue = value | (x => x is > 0 and < 100, "Value must be between 0 and 100");
```

### Chained Validations

```csharp
// Chain multiple conditions
int validatedValue = value
    | (x => x > 0)
    | (x => x < 100)
    | (x => x % 2 == 0);
```

### Choose Exception Type

```csharp
// Use ValidationResult for deferred exception selection
var result = value | (x => x >= 0);

// Throw specific exception types
result.OrThrowNull("paramName");           // ArgumentNullException
result.OrThrowInvalidOperation();          // InvalidOperationException
result.OrThrow<CustomException>();         // Custom exception
result.OrThrow(() => new MyException());   // Exception factory
```

### Built-in Validators

```csharp
// String validation
string name = input.EnsureNotNullOrEmpty();
string trimmed = input.EnsureNotNullOrWhiteSpace();
string sized = input.EnsureMinLength(3).EnsureMaxLength(50);

// Numeric validation
int positive = number | Validate.Positive;
int nonNegative = number | Validate.NonNegative;
int ranged = number | Validate.InRange(1, 100);

// Collection validation
var items = list.EnsureNotEmpty();
var sized = list.EnsureMinCount(1).EnsureMaxCount(10);

// Nullable validation
var notNull = nullableValue.EnsureNotNull();
```

## Building from Source

```bash
# Build
./build.sh compile    # Linux/macOS
./build.cmd compile   # Windows

# Test
./build.sh test

# Pack
./build.sh pack
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
