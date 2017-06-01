using System;
using System.Threading.Tasks;
using ExpressionCache.Core.Tests.TestHelpers;
using Moq;
using Shouldly;
using Xunit;

namespace ExpressionCache.Core.Tests
{
    [Collection("HelperCollection")]
    public class ExpressionCacheBaseTests
    {
        private readonly TestFunctionsFixture _testFunctions;

        public ExpressionCacheBaseTests(TestFunctionsFixture testFunctions)
        {
            _testFunctions = testFunctions;
        }

        [Fact]
        public void GetKey_FunctionWithoutParameters_ShouldReturnCacheKeyString()
        {
            using (var fixture = new ExpressionCacheBaseFixture())
            {
                var key = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithoutParameters());
                var asyncKey = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

                key.ShouldBe(
                    CacheKeyHelper.Format(_testFunctions.ClassName) +
                    CacheKeyHelper.Format(nameof(_testFunctions.FunctionWithoutParameters))
                );
                asyncKey.ShouldBe(
                    CacheKeyHelper.Format(_testFunctions.ClassName) +
                    CacheKeyHelper.Format(nameof(_testFunctions.FunctionWithoutParametersAsync))
                );
            }
        }

        [Fact]
        public void GetKey_FunctionWithOneParameter_ShouldReturnCacheKeyString()
        {
            const int parameter = 1;

            using (var fixture = new ExpressionCacheBaseFixture())
            {
                var key = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithOneParameter(parameter));
                var asyncKey = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithOneParameterAsync(parameter));

                key.ShouldBe(
                    CacheKeyHelper.Format(_testFunctions.ClassName) +
                    CacheKeyHelper.Format(nameof(_testFunctions.FunctionWithOneParameter)) +
                    CacheKeyHelper.Format(parameter)
                );
                asyncKey.ShouldBe(
                    CacheKeyHelper.Format(_testFunctions.ClassName) +
                    CacheKeyHelper.Format(nameof(_testFunctions.FunctionWithOneParameterAsync)) +
                    CacheKeyHelper.Format(parameter)
                );
            }
        }

        [Fact]
        public void GetKey_FunctionWithTwoParameters_ShouldReturnCacheKeyString()
        {
            const int parameterOne = 1;
            const string parameterTwo = "Testing";

            using (var fixture = new ExpressionCacheBaseFixture())
            {
                var key = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithTwoParameters(parameterOne, parameterTwo));
                var asyncKey = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithTwoParametersAsync(parameterOne, parameterTwo));

                key.ShouldBe(
                    CacheKeyHelper.Format(_testFunctions.ClassName) +
                    CacheKeyHelper.Format(nameof(_testFunctions.FunctionWithTwoParameters)) +
                    CacheKeyHelper.Format(parameterOne) +
                    CacheKeyHelper.Format(parameterTwo)
                );
                asyncKey.ShouldBe(
                    CacheKeyHelper.Format(_testFunctions.ClassName) +
                    CacheKeyHelper.Format(nameof(_testFunctions.FunctionWithTwoParametersAsync)) +
                    CacheKeyHelper.Format(parameterOne) +
                    CacheKeyHelper.Format(parameterTwo)
                );
            }
        }

        [Fact]
        public void GetKey_NonFunction_ShouldThrowArgumentException()
        {
            using (var fixture = new ExpressionCacheBaseFixture())
            {
                Should.Throw<ArgumentException>(() => fixture.ExpressionCacheBase.GetKey(() => 1 + 2));
            }
        }

        [Fact]
        public void InvokeCache_ResultInCache_ShouldReturnCachedResult()
        {
            using (var fixture = new ExpressionCacheBaseFixture())
            {
                fixture.SetCacheProviderGetSuccess();
                var key = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithoutParameters());

                var result = fixture.ExpressionCacheBase.InvokeCache(() => _testFunctions.FunctionWithoutParameters(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Invoke);

                result.ShouldBe(fixture.CachedResult);
                fixture.CacheProviderMock.Verify(m => m.Get<string>(key), Times.Once);
                fixture.CacheProviderMock.Verify(m => m.Set(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Never);
            }
        }

        [Fact]
        public async Task InvokeCacheAsync_ResultInCache_ShouldReturnCachedResult()
        {
            using (var fixture = new ExpressionCacheBaseFixture())
            {
                fixture.SetCacheProviderGetAsyncSuccess();
                var key = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

                var result = await fixture.ExpressionCacheBase.InvokeCacheAsync(() => _testFunctions.FunctionWithoutParametersAsync(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Invoke);

                result.ShouldBe(fixture.CachedResult);
                fixture.CacheProviderMock.Verify(m => m.GetAsync<string>(key), Times.Once);
                fixture.CacheProviderMock.Verify(m => m.SetAsync(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Never);
            }
        }

        [Fact]
        public void InvokeCache_ResultNotInCache_ShouldReturnNewResult()
        {
            using (var fixture = new ExpressionCacheBaseFixture())
            {
                fixture.SetCacheProviderGetFailure();
                var key = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithoutParameters());

                var result = fixture.ExpressionCacheBase.InvokeCache(() => _testFunctions.FunctionWithoutParameters(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Invoke);

                result.ShouldBe(_testFunctions.ReturnResult);
                fixture.CacheProviderMock.Verify(m => m.Get<string>(key), Times.Once);
                fixture.CacheProviderMock.Verify(m => m.Set(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Once);
            }
        }

        [Fact]
        public async Task InvokeCacheAsync_ResultNotInCache_ShouldReturnNewResult()
        {
            using (var fixture = new ExpressionCacheBaseFixture())
            {
                fixture.SetCacheProviderGetAsyncFailure();
                var key = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

                var result = await fixture.ExpressionCacheBase.InvokeCacheAsync(() => _testFunctions.FunctionWithoutParametersAsync(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Invoke);

                result.ShouldBe(_testFunctions.ReturnResult);
                fixture.CacheProviderMock.Verify(m => m.GetAsync<string>(key), Times.Once);
                fixture.CacheProviderMock.Verify(m => m.SetAsync(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Once);
            }
        }

        [Fact]
        public void InvokeCache_Overwrite_ShouldReturnFunctionResult()
        {
            using (var fixture = new ExpressionCacheBaseFixture())
            {
                fixture.SetCacheProviderGetSuccess();
                var key = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithoutParameters());

                var result = fixture.ExpressionCacheBase.InvokeCache(() => _testFunctions.FunctionWithoutParameters(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Overwrite);

                result.ShouldBe(_testFunctions.ReturnResult);
                fixture.CacheProviderMock.Verify(m => m.Get<string>(key), Times.Never);
                fixture.CacheProviderMock.Verify(m => m.Set(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Once);
            }
        }

        [Fact]
        public async Task InvokeCacheAsync_Overwrite_ShouldReturnFunctionResult()
        {
            using (var fixture = new ExpressionCacheBaseFixture())
            {
                fixture.SetCacheProviderGetAsyncSuccess();
                var key = fixture.ExpressionCacheBase.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

                var result = await fixture.ExpressionCacheBase.InvokeCacheAsync(() => _testFunctions.FunctionWithoutParametersAsync(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Overwrite);

                result.ShouldBe(_testFunctions.ReturnResult);
                fixture.CacheProviderMock.Verify(m => m.GetAsync<string>(key), Times.Never);
                fixture.CacheProviderMock.Verify(m => m.SetAsync(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Once);
            }
        }
    }
}

