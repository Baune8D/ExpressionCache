using Microsoft.Extensions.Caching.Memory;

namespace ExpressionCache.Core.Testing
{
    public class ExpressionCacheBaseWrapper : ExpressionCacheBase
    {
        public ExpressionCacheBaseWrapper(IMemoryCache internalCache, IExpressionCacheProvider provider) 
            : base(internalCache, provider) { }
    }
}
