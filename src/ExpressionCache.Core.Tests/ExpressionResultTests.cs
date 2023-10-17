using System;
using System.Linq.Expressions;
using ExpressionCache.Core.Tests.Data;
using FluentAssertions;
using Xunit;

namespace ExpressionCache.Core.Tests
{
    public class ExpressionResultTests : IClassFixture<TestFunctionsFixture>
    {
        private readonly TestFunctionsFixture _testFunctions;

        public ExpressionResultTests(TestFunctionsFixture testFunctions)
        {
            _testFunctions = testFunctions;
        }

        [Fact]
        public void NewBase_ExpressionResult_ShouldReturnNewBaseObject()
        {
            Expression<Func<string>> expression = () => _testFunctions.FunctionWithoutParameters();

            if (!(expression.Body is MethodCallExpression methodCall))
            {
                throw new InvalidOperationException(nameof(methodCall));
            }

            var expressionResult = new ExpressionResult(2, new object(), "TestBaseCacheKey")
            {
                Method = methodCall.Method,
                MethodCall = methodCall,
                Arguments =
                {
                    [0] = new object(),
                    [1] = new object(),
                },
                CacheKey = "CacheKey",
            };

            var newResult = expressionResult.NewBase();

            newResult.Instance.Should().Be(expressionResult.Instance);
            newResult.BaseCacheKey.Should().Be(expressionResult.BaseCacheKey);

            newResult.Arguments.Length.Should().Be(expressionResult.Arguments.Length);
            newResult.Arguments[0].Should().BeNull();
            newResult.Arguments[1].Should().BeNull();

            newResult.Method.Should().BeNull();
            newResult.MethodCall.Should().BeNull();
            newResult.CacheKey.Should().BeNull();
        }
    }
}
