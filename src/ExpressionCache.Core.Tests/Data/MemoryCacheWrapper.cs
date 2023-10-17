using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ExpressionCache.Core.Tests.Data
{
    public class MemoryCacheWrapper : MemoryCache
    {
        public MemoryCacheWrapper(IOptions<MemoryCacheOptions> optionsAccessor)
            : base(optionsAccessor)
        {
        }
    }
}
