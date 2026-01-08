using System;
using System.Linq.Expressions;
using AwesomeAssertions;
using ExpressionCache.Core.Tests.Data;
using Xunit;

namespace ExpressionCache.Core.Tests;

public class ExpressionResultTests(TestFunctionsFixture testFunctions) : IClassFixture<TestFunctionsFixture>
{
    [Fact]
    public void NewBase_ExpressionResult_ShouldReturnNewBaseObject()
    {
        Expression<Func<string>> expression = () => testFunctions.FunctionWithoutParameters();

        if (expression.Body is not MethodCallExpression methodCall)
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

        newResult.Arguments.Should().HaveCount(expressionResult.Arguments.Length);
        newResult.Arguments[0].Should().BeNull();
        newResult.Arguments[1].Should().BeNull();

        newResult.Method.Should().BeNull();
        newResult.MethodCall.Should().BeNull();
        newResult.CacheKey.Should().BeNull();
    }
}
