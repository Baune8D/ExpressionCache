using Microsoft.Extensions.Caching.Memory;

namespace ExpressionCache.Core.Tests.TestHelpers
{
    public class ExpressionCacheBaseWrapper : ExpressionCacheBase
    {
        public ExpressionCacheBaseWrapper(IMemoryCache internalCache, IExpressionCacheProvider provider) 
            : base(internalCache, provider) { }
    }
}
