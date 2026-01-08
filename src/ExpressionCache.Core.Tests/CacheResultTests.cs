using AwesomeAssertions;
using Xunit;

namespace ExpressionCache.Core.Tests;

public class CacheResultTests
{
    [Fact]
    public void Failure_Integer_ShouldReturnSuccessFalseWithContentAsDefaultInt()
    {
        var result = CacheResult<int>.Failure();
        result.Success.Should().BeFalse();
        result.Content.Should().Be(0);
    }
}
