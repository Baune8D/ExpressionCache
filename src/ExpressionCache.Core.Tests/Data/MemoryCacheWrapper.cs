using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ExpressionCache.Core.Tests.Data;

public class MemoryCacheWrapper(IOptions<MemoryCacheOptions> optionsAccessor)
    : MemoryCache(optionsAccessor);
