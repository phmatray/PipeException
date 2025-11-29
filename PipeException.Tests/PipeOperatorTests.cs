namespace PipeException.Tests;

public class PipeOperatorTests
{
    [Fact]
    public void Pipe_WhenConditionIsTrue_ReturnsOriginalValue()
    {
        const int value = 5;

        int result = value | (x => x > 0);

        Assert.Equal(5, result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(100)]
    public void Pipe_WithVariousValidValues_ReturnsValue(int value)
    {
        int result = value | (x => x >= 0);

        Assert.Equal(value, result);
    }

    [Fact]
    public void Pipe_WhenConditionIsFalse_ThrowsArgumentException()
    {
        const int value = -1;

        Assert.Throws<ArgumentException>(() => { int _ = value | (x => x >= 0); });
    }

    [Fact]
    public void Pipe_WhenConditionIsFalse_ExceptionContainsConditionNotMet()
    {
        const int value = -1;

        var ex = Assert.Throws<ArgumentException>(() => { int _ = value | (x => x >= 0); });

        Assert.Contains("Condition not met", ex.Message);
    }

    [Fact]
    public void Pipe_WithCustomMessage_WhenConditionIsTrue_ReturnsValue()
    {
        const int value = 50;

        int result = value | (x => x is > 0 and < 100, "Value must be between 0 and 100");

        Assert.Equal(50, result);
    }

    [Fact]
    public void Pipe_WithCustomMessage_WhenConditionIsFalse_ThrowsWithCustomMessage()
    {
        const int value = 150;
        const string customMessage = "Value must be between 0 and 100";

        var ex = Assert.Throws<ArgumentException>(() =>
        {
            int _ = value | (x => x is > 0 and < 100, customMessage);
        });

        Assert.Contains(customMessage, ex.Message);
    }

    [Fact]
    public void Pipe_MultipleConditions_AllPass_ReturnsValue()
    {
        const int value = 50;

        int result = value
            | (x => x > 0)
            | (x => x < 100)
            | (x => x % 2 == 0);

        Assert.Equal(50, result);
    }

    [Fact]
    public void Pipe_MultipleConditions_SecondFails_ThrowsException()
    {
        const int value = 150;

        Assert.Throws<ArgumentException>(() =>
        {
            int _ = value
                | (x => x > 0)
                | (x => x < 100);
        });
    }

    [Fact]
    public void Pipe_WithString_ValidatesCorrectly()
    {
        const string value = "hello";

        string result = value | (s => !string.IsNullOrEmpty(s));

        Assert.Equal("hello", result);
    }

    [Fact]
    public void Pipe_WithString_EmptyString_ThrowsException()
    {
        const string value = "";

        Assert.Throws<ArgumentException>(() => { string _ = value | (s => !string.IsNullOrEmpty(s)); });
    }

    [Fact]
    public void Pipe_WithReferenceType_ValidatesCorrectly()
    {
        var list = new List<int> { 1, 2, 3 };

        List<int> result = list | (l => l.Count > 0);

        Assert.Same(list, result);
    }

    [Fact]
    public void Pipe_WithReferenceType_EmptyCollection_ThrowsException()
    {
        var list = new List<int>();

        Assert.Throws<ArgumentException>(() => { List<int> _ = list | (l => l.Count > 0); });
    }

    // New tests for different exception types

    [Fact]
    public void Pipe_OrThrowInvalidOperation_ThrowsInvalidOperationException()
    {
        const int value = -1;

        Assert.Throws<InvalidOperationException>(() =>
        {
            var _ = (value | (x => x >= 0)).OrThrowInvalidOperation();
        });
    }

    [Fact]
    public void Pipe_OrThrowNull_ThrowsArgumentNullException()
    {
        const int value = -1;

        Assert.Throws<ArgumentNullException>(() =>
        {
            var _ = (value | (x => x >= 0)).OrThrowNull("value");
        });
    }

    [Fact]
    public void Pipe_OrThrowCustomFactory_ThrowsCustomException()
    {
        const int value = -1;

        var ex = Assert.Throws<InvalidOperationException>(() =>
        {
            var _ = (value | (x => x >= 0)).OrThrow(() => new InvalidOperationException("Custom error"));
        });

        Assert.Equal("Custom error", ex.Message);
    }
}
