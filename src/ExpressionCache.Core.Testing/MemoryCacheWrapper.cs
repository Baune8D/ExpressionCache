using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ExpressionCache.Core.Testing
{
    public class MemoryCacheWrapper : MemoryCache
    {
        public MemoryCacheWrapper(IOptions<MemoryCacheOptions> optionsAccessor) 
            : base(optionsAccessor) { }

        public new bool TryGetValue(object key, out object result)
        {
            result = null;
            return false;
        }
    }
}
