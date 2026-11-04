# ExpressionCache

[![Build](https://github.com/Baune8D/ExpressionCache/actions/workflows/pipeline.yml/badge.svg?branch=main)](https://github.com/Baune8D/ExpressionCache/actions/workflows/pipeline.yml)
[![codecov](https://codecov.io/gh/Baune8D/ExpressionCache/branch/main/graph/badge.svg)](https://codecov.io/gh/Baune8D/ExpressionCache)
[![License: MIT](https://img.shields.io/github/license/Baune8D/ExpressionCache)](https://github.com/Baune8D/ExpressionCache/blob/main/LICENSE)

| Package | Version | Downloads |
| --- | --- | --- |
| ExpressionCache.Core | [![NuGet Version](https://img.shields.io/nuget/v/ExpressionCache.Core)](https://www.nuget.org/packages/ExpressionCache.Core) | [![NuGet Downloads](https://img.shields.io/nuget/dt/ExpressionCache.Core)](https://www.nuget.org/packages/ExpressionCache.Core) |
| ExpressionCache.Distributed | [![NuGet Version](https://img.shields.io/nuget/v/ExpressionCache.Distributed)](https://www.nuget.org/packages/ExpressionCache.Distributed) | [![NuGet Downloads](https://img.shields.io/nuget/dt/ExpressionCache.Distributed)](https://www.nuget.org/packages/ExpressionCache.Distributed) |

**MyGet development feed:** [https://www.myget.org/F/baunegaard/api/v3/index.json](https://www.myget.org/F/baunegaard/api/v3/index.json)  

**Cache the results of .NET method calls, with cache keys built from the call itself.** ExpressionCache turns a method call expression into a key (type name, method name and argument values). It returns the cached result, or calls the method and caches what it returns. ExpressionCache.Distributed stores the results in any `IDistributedCache`, such as Redis or SQL Server.

```csharp
// Returns the cached product, or calls repository.GetAsync(42) and caches the result for an hour.
// Cache key: ExprCache{IProductRepository}{GetAsync}{42}
var product = await cache.InvokeCacheAsync(() => repository.GetAsync(42), TimeSpan.FromHours(1), CacheAction.Invoke);
```

## Features

- **Keys from method calls:** `() => repository.GetAsync(42)` becomes `ExprCache{IProductRepository}{GetAsync}{42}`. You don't write or maintain key strings.
- **A cache action per call:** read through the cache (`Invoke`), refresh an entry (`Overwrite`) or skip the cache (`Bypass`).
- **Any `IDistributedCache`:** Redis, SQL Server, in-memory and other implementations. Values are stored as JSON.
- **Entries by expression:** `Get`, `Set`, `Exists` and `Remove` (each with an async version) and `GetManyAsync` take the same expressions.
- **Your own types as arguments:** implement `ICacheKey` to choose the values that go into the key.
- **Other caches:** implement `IExpressionCacheProvider` and derive from `ExpressionCacheBase`.
- **Targets .NET 8, 9 and 10.**

## Getting started

These steps use ExpressionCache.Distributed. To use a cache that doesn't implement `IDistributedCache`, see [Writing a provider](#writing-a-provider).

### 1. Install

```bash
dotnet add package ExpressionCache.Distributed
```

This also installs ExpressionCache.Core. You also need an `IDistributedCache` implementation, for example Redis:

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

// Your service that uses the cache (step 3).
builder.Services.AddScoped<ProductService>();
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

Callers can pass their own cache action, e.g. `GetAsync(42, CacheAction.Overwrite)` to refresh the entry. See [Cache actions](#cache-actions).

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

## Cache keys

A key is `ExprCache` followed by one `{...}` part for each of these:

1. The type of the object the method is called on, without its namespace. This is the type of the expression in your code: `this` gives the class, a variable declared as an interface gives the interface. A static method has no object, so this part is `{0x6e756c6c}`.
2. The method name.
3. For a generic method, each type argument, with its namespace: `Find<Product>()` adds `{MyApp.Product}`.
4. Each argument value, in order.

| Expression | Key |
| --- | --- |
| `() => GetAsync(42, CacheAction.Bypass)` in `ProductService` | `ExprCache{ProductService}{GetAsync}{42}{Bypass}` |
| `() => repository.GetAsync(42)`, where `repository` is an `IProductRepository` | `ExprCache{IProductRepository}{GetAsync}{42}` |

`GetKey` returns the key for an expression without calling the method:

```csharp
string key = cache.GetKey(() => GetAsync(42, CacheAction.Bypass));
```

Argument values are converted as follows. The checks run in this order:

| Value | Key part | Example |
| --- | --- | --- |
| `null` | `CacheKeyBuilder.NullString` | `{0x6e756c6c}` |
| `DateTime` | Its `Ticks` | `{639029198450000000}` |
| `Guid` | `ToString()` | `{3f2504e0-4f89-11d3-9a0c-0305e82c3301}` |
| Other `IConvertible` types: numbers, `string`, `bool`, `char`, enums | `ToString(CultureInfo.InvariantCulture)` | `{42}`, `{5.5}`, `{books}`, `{True}`, `{Bypass}` |
| `IEnumerable`, e.g. arrays and lists | One part per element | `new[] { 1, 2 }` → `{1}{2}` |
| `ICacheKey` | The parts added by `BuildCacheKey` | See [Your own types as arguments](#your-own-types-as-arguments) |
| Anything else | Throws `ArgumentException` ("... cannot be a cache key") | `DateTimeOffset`, `TimeSpan`, `DateOnly`, records |

Because of the order, a `string` is one part (not one per character), and a type that implements both `IEnumerable` and `ICacheKey` is keyed by its elements.

### Your own types as arguments

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

### Keys that can collide

A key contains only the parts above, so these calls share a key:

- Calls on types with the same name in different namespaces. A generic type is named without its type arguments: `Repository<Product>` and `Repository<Order>` are both ``Repository`1``.
- Overloads called with the same values, e.g. `Find(5)` and `Find("5")`.
- Static methods with the same name and arguments in different classes, since none of them has a type part.
- Values that contain braces, since they aren't escaped: the keys for `Find("a}{b")` and `Find("a", "b")` both end in `{Find}{a}{b}`.

## IDistributedCacheService

`IDistributedCacheService` is in ExpressionCache.Distributed. It extends `IExpressionCacheBase` from ExpressionCache.Core, which has `GetKey`, `InvokeCache` and `InvokeCacheAsync`.

| Member | Returns | Description |
| --- | --- | --- |
| `GetKey(expression)` | `string` | The cache key for the call. |
| `InvokeCache(expression, expiry, cacheAction)` | `TResult` | Returns the cached result or calls the method, depending on the [cache action](#cache-actions). For methods that return `TResult`. |
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

## Limitations

- **The lambda must be a single method call,** such as `() => GetAsync(id, CacheAction.Bypass)`. Anything else, such as `() => GetName() ?? "none"`, throws `ArgumentException`. The arguments can be any expression.

## Writing a provider

ExpressionCache.Core has the key generation and the cache logic, and doesn't depend on a particular cache. To use another cache, implement `IExpressionCacheProvider` and derive from `ExpressionCacheBase`. For example, for `IMemoryCache`:

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

| Type | Description |
| --- | --- |
| `IExpressionCacheProvider` | Reads and writes values by key: `Get`, `GetAsync`, `Set` and `SetAsync`. `ExpressionCacheBase` only calls `Set` and `SetAsync` with non-null values. |
| `CacheResult<TResult>` | The result of a read. Return `CacheResult<TResult>.Failure()` on a miss, or `Success = true` with the value in `Content`. |
| `ExpressionCacheBase` | Abstract class that implements `IExpressionCacheBase` (`GetKey`, `InvokeCache`, `InvokeCacheAsync`) on top of a provider. Subclasses get the provider from the protected `Provider` property. |

`ExpressionCacheBase` is `IDisposable`. It keeps the type and method part of each key in an `IMemoryCache`, which it disposes when it is disposed. The constructor that takes only a provider creates that cache. The other constructor takes it as its first argument, so don't pass a cache you use for anything else.

ExpressionCache.Distributed is built the same way: see [`DistributedCacheProvider`](https://github.com/Baune8D/ExpressionCache/blob/main/src/ExpressionCache.Distributed/DistributedCacheProvider.cs) and [`DistributedCacheService`](https://github.com/Baune8D/ExpressionCache/blob/main/src/ExpressionCache.Distributed/DistributedCacheService.cs).

## Building from source

The build uses [NUKE](https://nuke.build/):

```bash
./build.sh Test        # Windows: .\build.cmd Test
```

## License

[MIT](https://github.com/Baune8D/ExpressionCache/blob/main/LICENSE)
