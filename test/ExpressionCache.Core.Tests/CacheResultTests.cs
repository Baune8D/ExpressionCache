using Shouldly;
using Xunit;

namespace ExpressionCache.Core.Tests
{
    public class CacheResultTests
    {
        [Fact]
        public void Failure_Integer_ShouldReturnSuccessFalseWithContentAsDefaultInt()
        {
            var result = CacheResult<int>.Failure();
            result.Success.ShouldBeFalse();
            result.Content.ShouldBe(default(int));
        }
    }
}
