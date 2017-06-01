using System;
using System.Linq.Expressions;
using ExpressionCache.Core.Tests.TestHelpers;
using Shouldly;
using Xunit;

namespace ExpressionCache.Core.Tests
{
    [Collection("HelperCollection")]
    public class ExpressionResultTests
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

            var methodCall = expression.Body as MethodCallExpression;
            if (methodCall == null) throw new NullReferenceException(nameof(methodCall));

            var expressionResult = new ExpressionResult(2, new object(), "TestBaseCacheKey")
            {
                Method = methodCall.Method,
                MethodCall = methodCall,
                Arguments =
                {
                    [0] = new object(),
                    [1] = new object()
                },
                CacheKey = "CacheKey"
            };

            var newResult = expressionResult.NewBase();

            newResult.Instance.ShouldBe(expressionResult.Instance);
            newResult.BaseCacheKey.ShouldBe(expressionResult.BaseCacheKey);

            newResult.Arguments.Length.ShouldBe(expressionResult.Arguments.Length);
            newResult.Arguments[0].ShouldBeNull();
            newResult.Arguments[1].ShouldBeNull();

            newResult.Method.ShouldBeNull();
            newResult.MethodCall.ShouldBeNull();
            newResult.CacheKey.ShouldBeNull();
        }
    }
}
