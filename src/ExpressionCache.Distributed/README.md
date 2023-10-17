# ExpressionCache.Distributed

ExpressionCache use providers to support different caching engines.

This package works with Microsofts IDistributedCache interface.

## .NET Core Dependency Injection
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

## Full service example
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

## Interface members
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
