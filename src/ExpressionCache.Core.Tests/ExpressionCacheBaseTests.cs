using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using ExpressionCache.Core.Tests.Data;
using Moq;
using Xunit;

namespace ExpressionCache.Core.Tests;

public class ExpressionCacheBaseTests(TestFunctionsFixture testFunctions) : IClassFixture<TestFunctionsFixture>
{
    [Fact]
    public void GetKey_FunctionWithoutParameters_ShouldReturnCacheKeyString()
    {
        using var fixture = new ExpressionCacheBaseFixture();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParameters());
        var asyncKey = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParametersAsync());

        key.Should().Be(CacheKeyHelper.Prefix(
            CacheKeyHelper.Format(testFunctions.ClassName) +
            CacheKeyHelper.Format(nameof(testFunctions.FunctionWithoutParameters))));
        asyncKey.Should().Be(CacheKeyHelper.Prefix(
            CacheKeyHelper.Format(testFunctions.ClassName) +
            CacheKeyHelper.Format(nameof(testFunctions.FunctionWithoutParametersAsync))));
    }

    [Fact]
    public void GetKey_FunctionWithOneParameter_ShouldReturnCacheKeyString()
    {
        const int parameter = 1;

        using var fixture = new ExpressionCacheBaseFixture();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithOneParameter(parameter));
        var asyncKey = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithOneParameterAsync(parameter));

        key.Should().Be(CacheKeyHelper.Prefix(
            CacheKeyHelper.Format(testFunctions.ClassName) +
            CacheKeyHelper.Format(nameof(testFunctions.FunctionWithOneParameter)) +
            CacheKeyHelper.Format(parameter)));
        asyncKey.Should().Be(CacheKeyHelper.Prefix(
            CacheKeyHelper.Format(testFunctions.ClassName) +
            CacheKeyHelper.Format(nameof(testFunctions.FunctionWithOneParameterAsync)) +
            CacheKeyHelper.Format(parameter)));
    }

    [Fact]
    public void GetKey_FunctionWithTwoParameters_ShouldReturnCacheKeyString()
    {
        const int parameterOne = 1;
        const string parameterTwo = "Testing";

        using var fixture = new ExpressionCacheBaseFixture();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithTwoParameters(parameterOne, parameterTwo));
        var asyncKey = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithTwoParametersAsync(parameterOne, parameterTwo));

        key.Should().Be(CacheKeyHelper.Prefix(
            CacheKeyHelper.Format(testFunctions.ClassName) +
            CacheKeyHelper.Format(nameof(testFunctions.FunctionWithTwoParameters)) +
            CacheKeyHelper.Format(parameterOne) +
            CacheKeyHelper.Format(parameterTwo)));
        asyncKey.Should().Be(CacheKeyHelper.Prefix(
            CacheKeyHelper.Format(testFunctions.ClassName) +
            CacheKeyHelper.Format(nameof(testFunctions.FunctionWithTwoParametersAsync)) +
            CacheKeyHelper.Format(parameterOne) +
            CacheKeyHelper.Format(parameterTwo)));
    }

    [Fact]
    public void GetKey_NonFunction_ShouldThrowArgumentException()
    {
        var act = () =>
        {
            using var fixture = new ExpressionCacheBaseFixture();
            fixture.ExpressionCacheBase.GetKey(() => 1 + 2);
        };

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void InvokeCache_ResultInCache_ShouldReturnCachedResult()
    {
        using var fixture = new ExpressionCacheBaseFixture();
        fixture.SetCacheProviderGetSuccess();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParameters());

        var result = fixture.ExpressionCacheBase.InvokeCache(() => testFunctions.FunctionWithoutParameters(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Invoke);

        result.Should().Be(ExpressionCacheBaseFixture.CachedResult);
        fixture.CacheProviderMock.Verify(m => m.Get<string>(key), Times.Once);
        fixture.CacheProviderMock.Verify(m => m.Set(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Never);
    }

    [Fact]
    public async Task InvokeCacheAsync_ResultInCache_ShouldReturnCachedResult()
    {
        using var fixture = new ExpressionCacheBaseFixture();
        fixture.SetCacheProviderGetAsyncSuccess();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParametersAsync());

        var result = await fixture.ExpressionCacheBase.InvokeCacheAsync(() => testFunctions.FunctionWithoutParametersAsync(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Invoke);

        result.Should().Be(ExpressionCacheBaseFixture.CachedResult);
        fixture.CacheProviderMock.Verify(m => m.GetAsync<string>(key), Times.Once);
        fixture.CacheProviderMock.Verify(m => m.SetAsync(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Never);
    }

    [Fact]
    public void InvokeCache_ResultNotInCache_ShouldCacheAndReturnResult()
    {
        using var fixture = new ExpressionCacheBaseFixture();
        fixture.SetCacheProviderGetFailure();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParameters());

        var result = fixture.ExpressionCacheBase.InvokeCache(() => testFunctions.FunctionWithoutParameters(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Invoke);

        result.Should().Be(TestFunctionsFixture.ReturnResult);
        fixture.CacheProviderMock.Verify(m => m.Get<string>(key), Times.Once);
        fixture.CacheProviderMock.Verify(m => m.Set(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Once);
    }

    [Fact]
    public async Task InvokeCacheAsync_ResultNotInCache_ShouldCacheAndReturnResult()
    {
        using var fixture = new ExpressionCacheBaseFixture();
        fixture.SetCacheProviderGetAsyncFailure();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParametersAsync());

        var result = await fixture.ExpressionCacheBase.InvokeCacheAsync(() => testFunctions.FunctionWithoutParametersAsync(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Invoke);

        result.Should().Be(TestFunctionsFixture.ReturnResult);
        fixture.CacheProviderMock.Verify(m => m.GetAsync<string>(key), Times.Once);
        fixture.CacheProviderMock.Verify(m => m.SetAsync(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Once);
    }

    [Fact]
    public void InvokeCache_NullFunction_ShouldNotCacheNull()
    {
        using var fixture = new ExpressionCacheBaseFixture();
        fixture.SetCacheProviderGetFailure();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParameters());

        var result = fixture.ExpressionCacheBase.InvokeCache(() => testFunctions.NullFunctionWithoutParameters(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Invoke);

        result.Should().BeNull();
        fixture.CacheProviderMock.Verify(m => m.Get<string>(key), Times.Never);
        fixture.CacheProviderMock.Verify(m => m.Set(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Never);
    }

    [Fact]
    public async Task InvokeCacheAsync_NullFunction_ShouldNotCacheNull()
    {
        using var fixture = new ExpressionCacheBaseFixture();
        fixture.SetCacheProviderGetAsyncFailure();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParametersAsync());

        var result = await fixture.ExpressionCacheBase.InvokeCacheAsync(() => testFunctions.NullFunctionWithoutParametersAsync(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Invoke);

        result.Should().BeNull();
        fixture.CacheProviderMock.Verify(m => m.GetAsync<string>(key), Times.Never);
        fixture.CacheProviderMock.Verify(m => m.SetAsync(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Never);
    }

    [Fact]
    public void InvokeCache_Overwrite_ShouldReturnNewValueAndCache()
    {
        using var fixture = new ExpressionCacheBaseFixture();
        fixture.SetCacheProviderGetSuccess();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParameters());

        var result = fixture.ExpressionCacheBase.InvokeCache(() => testFunctions.FunctionWithoutParameters(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Overwrite);

        result.Should().Be(TestFunctionsFixture.ReturnResult);
        fixture.CacheProviderMock.Verify(m => m.Get<string>(key), Times.Never);
        fixture.CacheProviderMock.Verify(m => m.Set(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Once);
    }

    [Fact]
    public async Task InvokeCacheAsync_Overwrite_ShouldReturnNewValueAndCache()
    {
        using var fixture = new ExpressionCacheBaseFixture();
        fixture.SetCacheProviderGetAsyncSuccess();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParametersAsync());

        var result = await fixture.ExpressionCacheBase.InvokeCacheAsync(() => testFunctions.FunctionWithoutParametersAsync(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Overwrite);

        result.Should().Be(TestFunctionsFixture.ReturnResult);
        fixture.CacheProviderMock.Verify(m => m.GetAsync<string>(key), Times.Never);
        fixture.CacheProviderMock.Verify(m => m.SetAsync(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Once);
    }

    [Fact]
    public void InvokeCache_Bypass_ShouldReturnNewValueAndCache()
    {
        using var fixture = new ExpressionCacheBaseFixture();
        fixture.SetCacheProviderGetSuccess();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParameters());

        var result = fixture.ExpressionCacheBase.InvokeCache(() => testFunctions.FunctionWithoutParameters(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Bypass);

        result.Should().Be(TestFunctionsFixture.ReturnResult);
        fixture.CacheProviderMock.Verify(m => m.Get<string>(key), Times.Never);
        fixture.CacheProviderMock.Verify(m => m.Set(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Never);
    }

    [Fact]
    public async Task InvokeCacheAsync_Bypass_ShouldReturnNewValueAndCache()
    {
        using var fixture = new ExpressionCacheBaseFixture();
        fixture.SetCacheProviderGetAsyncSuccess();
        var key = fixture.ExpressionCacheBase.GetKey(() => testFunctions.FunctionWithoutParametersAsync());

        var result = await fixture.ExpressionCacheBase.InvokeCacheAsync(() => testFunctions.FunctionWithoutParametersAsync(), ExpressionCacheBaseFixture.TimeSpan, CacheAction.Bypass);

        result.Should().Be(TestFunctionsFixture.ReturnResult);
        fixture.CacheProviderMock.Verify(m => m.GetAsync<string>(key), Times.Never);
        fixture.CacheProviderMock.Verify(m => m.SetAsync(key, result, ExpressionCacheBaseFixture.TimeSpan), Times.Never);
    }
}
