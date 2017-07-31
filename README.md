# ExpressionCache
[![Build status](https://ci.appveyor.com/api/projects/status/qmxfk6hhua5ev2cn?svg=true)](https://ci.appveyor.com/project/Baune8D/expressioncache)
[![codecov](https://codecov.io/gh/Baune8D/expressioncache/branch/master/graph/badge.svg)](https://codecov.io/gh/Baune8D/expressioncache)

NuGet feed: [https://www.myget.org/F/baunegaard/api/v3/index.json](https://www.myget.org/F/baunegaard/api/v3/index.json)

Inspired from this article [http://www.codeducky.org/robust-net-caching/](http://www.codeducky.org/robust-net-caching/)

## ExpressionCache.Core

### Key generation
The cache key will be a combination of class name, function name and parameter values.  

The following code snippet is the heart of the library:
```csharp
public class SampleService : ISampleService
{
    private readonly IDistributedCacheService _cacheService;
    private readonly ISampleRepository _sampleRepository;

    public SampleService(IDistributedCacheService cacheService, ISampleRepository sampleRepository)
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

### CacheAction enum
1. **Invoke** - The default action. Will check for a cached value and return it if found.
2. **Bypass** - Used to bypass the caching entirely. **Note!** Should always be used in the expression to InvokeCache.
3. **Overwrite** - Skip checking for cached value, but still cache new value.

### Object parameters
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

## ExpressionCache.Distributed
ExpressionCache use providers to support different caching engines.

Included in this repository is ExpressionCache.Distributed which works with Microsofts IDistributedCache interface.

### .NET Core Dependency Injection
The library include extensions for IServiceCollection interface.
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Register an IDistributedCache implementation.
    services.AddDistributedRedisCache(options =>
    {
        options.Configuration = "localhost";
        options.InstanceName = "Sample";
    });

    // Register the distributed cache service.
    services.AddDistributedExpressionCache();
}
```
You can now inject IDistributedCacheService using DI into you code.

### Full service example
```csharp
public class SampleService : ISampleService
{
    private readonly IDistributedCacheService _cacheService;
    private readonly ISampleRepository _sampleRepository;

    public SampleService(IDistributedCacheService cacheService, ISampleRepository sampleRepository)
    {
        _cacheService = cacheService;
        _sampleRepository = sampleRepository;
    }

    public async Task<bool> CreateAsync(EntityDto entity)
    {
        var newId = await _sampleRepository.CreateAsync(entity);
        if (newId > 0)
        {
            // Add new entity to cache.
            return await GetAsync(newId, CacheAction.Overwrite) != null;
        }
        return false;
    }

    public async Task<bool> UpdateAsync(EntityDto entity)
    {
        if (await _sampleRepository.UpdateAsync(entity))
        {
            // Overwrite cached entity with updated one.
            return await GetAsync(entity.Id, CacheAction.Overwrite) != null;
        }
        return false;
    }

    public async Task<bool> DeleteAsync(int entityId)
    {
        if (await _sampleRepository.DeleteAsync(id)) 
        {
            // Remove cached entity.
            await _cacheService.RemoveAsync(() => GetAsync(entityId, CacheAction.Bypass));
            return true;
        }
        return false;
    }

    public async Task<EntityModel> GetAsync(int entityId, CacheAction cacheAction = CacheAction.Invoke)
    {
        if (cacheAction != CacheAction.Bypass)
        {
            // Check for existing cached entity.
            return await _cacheService.InvokeCacheAsync(
                () => GetAsync(entityId, CacheAction.Bypass),
                TimeSpan.FromDays(1), cacheAction);
        }

        // If not already cached, this result will get cached.
        return await _sampleRepository.GetAsync(entityId);
    }
}
```

### Interface members
The full interface of IDistributedCacheService.  

```csharp
public interface IExpressionCacheBase
{
    string GetKey<TResult>(Expression<Func<TResult>> expression);
    string GetKey<TResult>(Expression<Func<Task<TResult>>> expression);

    TResult InvokeCache<TResult>(Expression<Func<TResult>> expression, TimeSpan expiry, CacheAction cacheAction);
    Task<TResult> InvokeCacheAsync<TResult>(Expression<Func<Task<TResult>>> expression, TimeSpan expiry, CacheAction cacheAction);
}

public interface IDistributedCacheService : IExpressionCacheBase
{
    void Remove<TResult>(Expression<Func<TResult>> expression);
    void Remove<TResult>(Expression<Func<Task<TResult>>> expression);
    Task RemoveAsync<TResult>(Expression<Func<TResult>> expression);
    Task RemoveAsync<TResult>(Expression<Func<Task<TResult>>> expression);

    bool Exists<TResult>(Expression<Func<TResult>> expression);
    bool Exists<TResult>(Expression<Func<Task<TResult>>> expression);
    Task<bool> ExistsAsync<TResult>(Expression<Func<TResult>> expression);
    Task<bool> ExistsAsync<TResult>(Expression<Func<Task<TResult>>> expression);

    TResult Get<TResult>(Expression<Func<TResult>> expression);
    TResult Get<TResult>(Expression<Func<Task<TResult>>> expression);
    Task<TResult> GetAsync<TResult>(Expression<Func<TResult>> expression);
    Task<TResult> GetAsync<TResult>(Expression<Func<Task<TResult>>> expression);

    Task<List<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<TResult>>> expressions);
    Task<List<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<Task<TResult>>>> expressions);

    void Set<TResult, TValue>(Expression<Func<TResult>> expression, TValue value, TimeSpan expiry);
    void Set<TResult, TValue>(Expression<Func<Task<TResult>>> expression, TValue value, TimeSpan expiry);
    Task SetAsync<TResult, TValue>(Expression<Func<TResult>> expression, TValue value, TimeSpan expiry);
    Task SetAsync<TResult, TValue>(Expression<Func<Task<TResult>>> expression, TValue value, TimeSpan expiry);
}
```