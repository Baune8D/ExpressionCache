# ExpressionCache.Core

**Cache keys from .NET method call expressions, and the base classes for ExpressionCache providers.** This package turns a method call expression into a key (type name, method name and argument values), and returns the cached result or calls the method and caches what it returns. The cache itself comes from a provider.

```csharp
// Cache key: ExprCache{ProductService}{GetAsync}{42}{Bypass}
string key = cache.GetKey(() => GetAsync(42, CacheAction.Bypass));
```

To cache in `IDistributedCache` (Redis, SQL Server, in-memory and others), install [ExpressionCache.Distributed](https://www.nuget.org/packages/ExpressionCache.Distributed) instead. It includes this package and has a getting started guide.

Full documentation: [github.com/Baune8D/ExpressionCache](https://github.com/Baune8D/ExpressionCache)

## Contents

| Type | Description |
| --- | --- |
| `IExpressionCacheBase` | `GetKey`, `InvokeCache` and `InvokeCacheAsync`. The interface you call. |
| `ExpressionCacheBase` | Abstract class that implements `IExpressionCacheBase` on top of an `IExpressionCacheProvider`. |
| `IExpressionCacheProvider` | Reads and writes values by key. Implement it for your cache. |
| `CacheResult<TResult>` | The result of a provider read: `Success` and `Content`. |
| `CacheAction` | `Invoke`, `Overwrite` or `Bypass`. See [Cache actions](https://github.com/Baune8D/ExpressionCache#cache-actions). |
| `ICacheKey`, `ICacheKeyBuilder`, `CacheKeyBuilder` | Let your own types be used as arguments. |

| Member of `IExpressionCacheBase` | Returns | Description |
| --- | --- | --- |
| `GetKey(expression)` | `string` | The cache key for the call. Has overloads for `Expression<Func<TResult>>` and `Expression<Func<Task<TResult>>>`. |
| `InvokeCache(expression, expiry, cacheAction)` | `TResult` | Returns the cached result or calls the method, depending on the cache action. Takes an `Expression<Func<TResult>>`. |
| `InvokeCacheAsync(expression, expiry, cacheAction)` | `Task<TResult>` | The same, for methods that return `Task<TResult>`. Takes an `Expression<Func<Task<TResult>>>`. |

## Cache keys

A key is `ExprCache` followed by one `{...}` part for each of these:

1. The type of the object the method is called on, without its namespace. This is the type of the expression in your code: `this` gives the class, a variable declared as an interface gives the interface. A static method has no object, so this part is `{0x6e756c6c}`.
2. The method name.
3. For a generic method, each type argument, with its namespace: `Find<Product>()` adds `{MyApp.Product}`.
4. Each argument value, in order.

Argument values are converted as follows. The checks run in this order:

| Value | Key part | Example |
| --- | --- | --- |
| `null` | `CacheKeyBuilder.NullString` | `{0x6e756c6c}` |
| `DateTime` | Its `Ticks` | `{639029198450000000}` |
| `Guid` | `ToString()` | `{3f2504e0-4f89-11d3-9a0c-0305e82c3301}` |
| Other `IConvertible` types: numbers, `string`, `bool`, `char`, enums | `ToString(CultureInfo.InvariantCulture)` | `{42}`, `{5.5}`, `{books}`, `{True}`, `{Bypass}` |
| `IEnumerable`, e.g. arrays and lists | One part per element | `new[] { 1, 2 }` → `{1}{2}` |
| `ICacheKey` | The parts added by `BuildCacheKey` | See below |
| Anything else | Throws `ArgumentException` ("... cannot be a cache key") | `DateTimeOffset`, `TimeSpan`, `DateOnly`, records |

Implement `ICacheKey` to use your own type as an argument. `BuildCacheKey` adds the values that identify an instance, converted as in the table above:

```csharp
using ExpressionCache.Core;

public sealed class ProductQuery : ICacheKey
{
    public string? Category { get; init; }

    public int Page { get; init; }

    public void BuildCacheKey(ICacheKeyBuilder builder)
    {
        builder
            .By(Category)
            .By(Page);
    }
}
```

In `ProductService`, `() => SearchAsync(new ProductQuery { Category = "books", Page = 2 }, CacheAction.Bypass)` gets the key `ExprCache{ProductService}{SearchAsync}{books}{2}{Bypass}`.

A key contains only these parts, so some calls share a key, e.g. overloads called with the same values. See [Keys that can collide](https://github.com/Baune8D/ExpressionCache#keys-that-can-collide).

## Writing a provider

To use a cache, implement `IExpressionCacheProvider` and derive from `ExpressionCacheBase`. For example, for `IMemoryCache`:

```csharp
using ExpressionCache.Core;
using Microsoft.Extensions.Caching.Memory;

public sealed class MemoryCacheProvider(IMemoryCache cache) : IExpressionCacheProvider
{
    public CacheResult<TResult> Get<TResult>(string key)
    {
        return cache.TryGetValue(key, out TResult? value)
            ? new CacheResult<TResult> { Success = true, Content = value }
            : CacheResult<TResult>.Failure();
    }

    public Task<CacheResult<TResult>> GetAsync<TResult>(string key)
    {
        return Task.FromResult(Get<TResult>(key));
    }

    public void Set<TResult>(string key, TResult value, TimeSpan expiry)
    {
        cache.Set(key, value, expiry);
    }

    public Task SetAsync<TResult>(string key, TResult value, TimeSpan expiry)
    {
        Set(key, value, expiry);
        return Task.CompletedTask;
    }
}

public sealed class MemoryExpressionCache(IMemoryCache cache)
    : ExpressionCacheBase(new MemoryCacheProvider(cache));
```

Register it as a singleton and inject `IExpressionCacheBase`:

```csharp
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IExpressionCacheBase, MemoryExpressionCache>();
```

- Return `CacheResult<TResult>.Failure()` on a miss, or `Success = true` with the value in `Content`.
- `ExpressionCacheBase` only calls `Set` and `SetAsync` with non-null values.
- Subclasses get the provider from the protected `Provider` property.
- `ExpressionCacheBase` is `IDisposable`. It keeps the type and method part of each key in an `IMemoryCache`, which it disposes when it is disposed. The constructor that takes only a provider creates that cache. The other constructor takes it as its first argument, so don't pass a cache you use for anything else.

ExpressionCache.Distributed is built the same way: see [`DistributedCacheProvider`](https://github.com/Baune8D/ExpressionCache/blob/main/src/ExpressionCache.Distributed/DistributedCacheProvider.cs) and [`DistributedCacheService`](https://github.com/Baune8D/ExpressionCache/blob/main/src/ExpressionCache.Distributed/DistributedCacheService.cs).

## Limitations

- **The lambda must be a single method call,** such as `() => GetAsync(id, CacheAction.Bypass)`. Anything else, such as `() => GetName() ?? "none"`, throws `ArgumentException`. The arguments can be any expression.

## License

[MIT](https://github.com/Baune8D/ExpressionCache/blob/main/LICENSE)
