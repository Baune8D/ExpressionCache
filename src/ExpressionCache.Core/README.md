# ExpressionCache.Core

ExpressionCache use providers to support different caching engines.

See `ExpressionCache.Distributed` for a cache provider for `IDistributedCache`.

## Key generation
The cache key will be a combination of class name, function name and parameter values.  

The following code snippet is the heart of the library:
```csharp
public class SampleService
{
    private readonly IDistributedCacheService _cacheService;
    private readonly SampleRepository _sampleRepository;

    public SampleService(IDistributedCacheService cacheService, SampleRepository sampleRepository)
    {
        _cacheService = cacheService;
        _sampleRepository = sampleRepository;
    }

    public async Task<EntityModel> GetAsync(int entityId, CacheAction cacheAction = CacheAction.Invoke)
    {
        if (cacheAction != CacheAction.Bypass)
        {
            return await _cacheService.InvokeCacheAsync(
                () => GetAsync(entityId, CacheAction.Bypass),
                TimeSpan.FromDays(1), cacheAction);
        }

        return await _sampleRepository.GetAsync(entityId);
    }
}
```

**Flow (Without an existing cache entry)**
1. Lets say we call GetAsync(5) on the above snippet.
2. cacheAction is not equal to Bypass, so InvokeCacheAsync gets called.
3. A lookup in cache with generated key \{SampleService}\{GetAsync}\{5}\{Bypass} happens.
4. No cache entry is found so the expression GetAsync(5, CacheAction.Bypass) is invoked.
5. This time the cache is skipped because of Bypass. Result of sampleRepository is returned.
6. The returned value gets cached and InvokeCacheAsync returns.

## CacheAction enum
1. **Invoke** - The default action. Will check for a cached value and return it if found.
2. **Bypass** - Used to bypass the caching entirely. **Note!** Should always be used in the expression to InvokeCache.
3. **Overwrite** - Skip checking for cached value, but still cache new value.

## Object parameters
If complex objects are used as function parameters, ExpressionCache needs a way to know how to build the key value.

By extending **ICacheKey** one can define how to build the key.

```csharp
public class SampleObject : ICacheKey
{
    public int Parameter1 { get; set; }
    public int Parameter2 { get; set; }

    public virtual void BuildCacheKey(ICacheKeyBuilder builder)
    {
        builder
            .By(Parameter1)
            .By(Parameter2);
    }
}
```
