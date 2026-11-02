# ExpressionCache.Distributed

**Cache the results of .NET method calls in `IDistributedCache`, with cache keys built from the call itself.** This package provides `IDistributedCacheService`. It turns a method call expression into a key (type name, method name and argument values), and returns the cached result or calls the method and caches what it returns. It works with any `IDistributedCache`, such as Redis or SQL Server.

```csharp
// Returns the cached product, or calls repository.GetAsync(42) and caches the result for an hour.
// Cache key: ExprCache{IProductRepository}{GetAsync}{42}
var product = await cache.InvokeCacheAsync(() => repository.GetAsync(42), TimeSpan.FromHours(1), CacheAction.Invoke);
```

Full documentation: [github.com/Baune8D/ExpressionCache](https://github.com/Baune8D/ExpressionCache)

## Getting started

### 1. Install

```bash
dotnet add package ExpressionCache.Distributed
```

This also installs [ExpressionCache.Core](https://www.nuget.org/packages/ExpressionCache.Core). You also need an `IDistributedCache` implementation, for example Redis:

```bash
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

### 2. Register services

`Program.cs`:

```csharp
using ExpressionCache.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Any IDistributedCache implementation. For development and tests, AddDistributedMemoryCache() also works.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost";
    options.InstanceName = "MyApp:";
});

builder.Services.AddDistributedExpressionCache();

// Your service that uses the cache (step 3). Register it as a singleton, see Limitations.
builder.Services.AddSingleton<ProductService>();
```

`AddDistributedExpressionCache()` registers `IDistributedCacheService` as a singleton, unless one is already registered. It uses the `IDistributedCache` you registered.

### 3. Cache a method

Give the method a `CacheAction` parameter, and let it call itself through the cache with `CacheAction.Bypass`:

```csharp
using ExpressionCache.Core;
using ExpressionCache.Distributed;

public class ProductService(IDistributedCacheService cache, IProductRepository repository)
{
    public Task<Product?> GetAsync(int id, CacheAction cacheAction = CacheAction.Invoke)
    {
        if (cacheAction != CacheAction.Bypass)
        {
            return cache.InvokeCacheAsync(() => GetAsync(id, CacheAction.Bypass), TimeSpan.FromHours(1), cacheAction);
        }

        return repository.GetAsync(id);
    }
}
```

When you call `GetAsync(42)`:

1. `cacheAction` is `Invoke`, so the call goes through `InvokeCacheAsync`.
2. The key is built from the expression: `ExprCache{ProductService}{GetAsync}{42}{Bypass}`.
3. If the key is in the cache, the cached product is returned.
4. If not, `GetAsync(42, CacheAction.Bypass)` is called. It skips the cache and reads from the repository. The result is cached for an hour and returned.

Always pass `CacheAction.Bypass` in the expression. The argument is part of the key, so every caller gets the same key, and the inner call reads from the repository instead of going back to the cache.

Callers can pass their own cache action, e.g. `GetAsync(42, CacheAction.Overwrite)` to refresh the entry.

### 4. Update and remove entries

Add these to `ProductService`:

```csharp
public async Task UpdateAsync(Product product)
{
    await repository.UpdateAsync(product);

    // Read the product again and replace the cached entry.
    await GetAsync(product.Id, CacheAction.Overwrite);
}

public async Task DeleteAsync(int id)
{
    await repository.DeleteAsync(id);

    // Use the same expression as in GetAsync, so the key matches.
    await cache.RemoveAsync(() => GetAsync(id, CacheAction.Bypass));
}
```

## Cache actions

`InvokeCache` and `InvokeCacheAsync` take a `CacheAction`:

| Value | Reads the cache | Calls the method | Caches the result |
| --- | --- | --- | --- |
| `Invoke` | Yes | Only on a miss | Only on a miss |
| `Overwrite` | No | Yes | Yes |
| `Bypass` | No | Yes | No |

A `null` result is never cached, so the method is called again the next time.

## IDistributedCacheService

`IDistributedCacheService` extends `IExpressionCacheBase` from ExpressionCache.Core, which has `GetKey`, `InvokeCache` and `InvokeCacheAsync`.

| Member | Returns | Description |
| --- | --- | --- |
| `GetKey(expression)` | `string` | The cache key for the call. |
| `InvokeCache(expression, expiry, cacheAction)` | `TResult` | Returns the cached result or calls the method, depending on the cache action. For methods that return `TResult`. |
| `InvokeCacheAsync(expression, expiry, cacheAction)` | `Task<TResult>` | The same, for methods that return `Task<TResult>`. |
| `Get(expression)` | `TResult?` | The cached value, or `default` on a miss. |
| `GetAsync(expression)` | `Task<TResult?>` | Async version of `Get`. |
| `GetManyAsync(expressions)` | `Task<List<TResult?>>` | The cached values for several calls, in the same order, with `default` for each miss. |
| `Set(expression, value, expiry)` | `void` | Caches `value` under the call's key. |
| `SetAsync(expression, value, expiry)` | `Task` | Async version of `Set`. |
| `Exists(expression)` | `bool` | Whether the call's key is in the cache. |
| `ExistsAsync(expression)` | `Task<bool>` | Async version of `Exists`. |
| `Remove(expression)` | `void` | Removes the call's entry. |
| `RemoveAsync(expression)` | `Task` | Async version of `Remove`. |

- `expression` is an `Expression<Func<TResult>>` for a method that returns `TResult`, or an `Expression<Func<Task<TResult>>>` for a method that returns `Task<TResult>`. All members except `InvokeCache` and `InvokeCacheAsync` have both overloads. For an async method, `TResult` is the task's result type, so `Get(() => GetAsync(42, CacheAction.Bypass))` returns a `Product?`.
- Only `InvokeCache` and `InvokeCacheAsync` call the method. The other members use the expression only to build the key.
- For a value type, the `default` that `Get`, `GetAsync` and `GetManyAsync` return on a miss looks the same as a cached `0` or `false`. Use `Exists` to tell them apart.
- `Set` and `SetAsync` take the value as a separate type parameter, `TValue`, so it isn't checked against the method's return type.
- `expiry` is absolute: the entry expires that long after it is written. It must be positive.
- Values are serialized to JSON with `System.Text.Json` (default options) and stored with `IDistributedCache.SetString`. Cached types must round-trip through `JsonSerializer`.

## Cache keys

A key is `ExprCache` followed by the type of the object the method is called on (without its namespace), the method name and each argument value, each in braces:

| Expression | Key |
| --- | --- |
| `() => GetAsync(42, CacheAction.Bypass)` in `ProductService` | `ExprCache{ProductService}{GetAsync}{42}{Bypass}` |
| `() => repository.GetAsync(42)`, where `repository` is an `IProductRepository` | `ExprCache{IProductRepository}{GetAsync}{42}` |

Numbers, strings, booleans, enums, `DateTime`, `Guid`, `null` and collections of these can be used as arguments. Other types must implement `ICacheKey`, otherwise building the key throws `ArgumentException`. See [Cache keys](https://github.com/Baune8D/ExpressionCache#cache-keys) for how each value is converted, how to implement `ICacheKey`, and which calls can share a key.

## Limitations

- **The object the method is called on is captured once.** ExpressionCache parses each lambda once and keeps the result, including the object the method is called on (`this` or `repository` in the examples above). Later calls of the same lambda reuse that object, even when they come from another instance. Use ExpressionCache in singleton services: in a scoped or transient service, `InvokeCache` and `InvokeCacheAsync` call the method on the instance that made the first call. Keys aren't affected, and argument values are read on every call.
- **Generic methods aren't supported.** Building a key for a call to a generic method throws `ArgumentException`.
- **The lambda must be a single method call,** such as `() => GetAsync(id, CacheAction.Bypass)`. Anything else, such as `() => GetName() ?? "none"`, throws `ArgumentException`. The arguments can be any expression.

## License

[MIT](https://github.com/Baune8D/ExpressionCache/blob/main/LICENSE.txt)
