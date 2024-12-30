using System;
using System.Collections.Generic;
using System.Globalization;
using ExpressionCache.Core.Tests.Data;
using FluentAssertions;
using Xunit;

namespace ExpressionCache.Core.Tests;

public class CacheKeyBuilderTests
{
    private readonly CacheKeyBuilder _cacheKeyBuilder = new();

    [Fact]
    public void By_Null_ShouldBeCacheKeyString()
    {
        var key = _cacheKeyBuilder.By(null).ToString();
        key.Should().Be(CacheKeyHelper.Format(CacheKeyBuilder.NullString));
    }

    [Fact]
    public void By_DateTime_ShouldBeCacheKeyString()
    {
        var dateTime = default(DateTime);
        var key = _cacheKeyBuilder.By(dateTime).ToString();
        key.Should().Be(CacheKeyHelper.Format(dateTime.Ticks));
    }

    [Fact]
    public void By_Guid_ShouldBeCacheKeyString()
    {
        var guid = Guid.NewGuid();
        var key = _cacheKeyBuilder.By(guid).ToString();
        key.Should().Be(CacheKeyHelper.Format(guid));
    }

    [Fact]
    public void By_Integer_ShouldBeCacheKeyString()
    {
        const int number = 5;
        var key = _cacheKeyBuilder.By(number).ToString();
        key.Should().Be(CacheKeyHelper.Format(number));
    }

    [Fact]
    public void By_Double_ShouldBeCacheKeyString()
    {
        const double number = 5.5;
        var key = _cacheKeyBuilder.By(number).ToString();
        key.Should().Be(CacheKeyHelper.Format(number.ToString(CultureInfo.InvariantCulture)));
    }

    [Fact]
    public void By_String_ShouldBeCacheKeyString()
    {
        const string text = "test";
        var key = _cacheKeyBuilder.By(text).ToString();
        key.Should().Be(CacheKeyHelper.Format(text));
    }

    [Fact]
    public void By_CacheableObject_ShouldBeCacheKeyString()
    {
        const int number = 5;
        const string text = "test";
        var obj = new CacheableObject(number, text);
        var key = _cacheKeyBuilder.By(obj).ToString();
        key.Should().Be(CacheKeyHelper.Format(number) + CacheKeyHelper.Format(text));
    }

    [Fact]
    public void By_NonCacheableObject_ShouldThrowArgumentException()
    {
        var obj = new NonCacheableObject();
        Action act = () => _cacheKeyBuilder.By(obj);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void By_List_ShouldBeCacheKeyString()
    {
        const string text1 = "test1";
        const string text2 = "test2";
        var list = new List<string> { text1, text2 };
        var key = _cacheKeyBuilder.By(list).ToString();
        key.Should().Be(CacheKeyHelper.Format(text1) + CacheKeyHelper.Format(text2));
    }
}
