namespace ExpressionCache.Core.Tests.Data;

public static class CacheKeyHelper
{
    public static string Prefix(string key)
    {
        return ExpressionCacheBase.CachePrefix + key;
    }

    public static string Format<TResult>(TResult value)
    {
        return "{" + value + "}";
    }
}
