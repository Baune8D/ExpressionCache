using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionCache.Core;

public sealed class ExpressionResult
{
    private readonly int _argumentCount;

    public ExpressionResult(int argumentCount, object instance, string baseCacheKey)
    {
        _argumentCount = argumentCount;

        Instance = instance;
        BaseCacheKey = baseCacheKey;

        Arguments = new object[_argumentCount]; // Values should not be cached
        Method = null; // Should not be cached
        MethodCall = null; // Should not be cached
        CacheKey = null; // Should not be cached
    }

    public object Instance { get; }

    public string BaseCacheKey { get; }

    [SuppressMessage("Microsoft.Performance", "CA1819", Justification = "Needs to be modified")]
    public object[] Arguments { get; }

    public MethodInfo Method { get; set; }

    public MethodCallExpression MethodCall { get; set; }

    public string CacheKey { get; set; }

    public ExpressionResult NewBase()
    {
        return new ExpressionResult(_argumentCount, Instance, BaseCacheKey);
    }
}
