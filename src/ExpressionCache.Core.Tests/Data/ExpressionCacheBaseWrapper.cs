using Microsoft.Extensions.Caching.Memory;

namespace ExpressionCache.Core.Tests.Data;

public class ExpressionCacheBaseWrapper : ExpressionCacheBase
{
    public ExpressionCacheBaseWrapper(IMemoryCache internalCache, IExpressionCacheProvider provider)
        : base(internalCache, provider)
    {
    }
}
