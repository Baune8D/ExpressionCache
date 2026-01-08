using Microsoft.Extensions.Caching.Memory;

namespace ExpressionCache.Core.Tests.Data;

public class ExpressionCacheBaseWrapper(IMemoryCache internalCache, IExpressionCacheProvider provider)
    : ExpressionCacheBase(internalCache, provider);
