using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace ExpressionCache.Distributed.Tests
{
    public class DistributedCacheProviderTests
    {
        private const string Key = "TestKey";
        private const string Value = "TestValue";

        private readonly MemoryDistributedCache _memoryDistributedCache;
        private readonly DistributedCacheProvider _distributedCacheProvider;

        public DistributedCacheProviderTests()
        {
            var options = new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
            _memoryDistributedCache = new MemoryDistributedCache(options);
            _distributedCacheProvider = new DistributedCacheProvider(_memoryDistributedCache);
        }

        [Fact]
        public void Get_String_ShouldReturnCacheResult()
        {
            _memoryDistributedCache.SetString(Key, JsonConvert.SerializeObject(Value));

            var cached = _distributedCacheProvider.Get<string>(Key);

            cached.Success.ShouldBeTrue();
            cached.Content.ShouldBe(Value);
        }

        [Fact]
        public void Get_String_ShouldReturnFailure()
        {
            var cached = _distributedCacheProvider.Get<string>(Key);

            cached.Success.ShouldBeFalse();
            cached.Content.ShouldBe(default);
        }

        [Fact]
        public async Task GetAsync_String_ShouldReturnCacheResult()
        {
            await _memoryDistributedCache.SetStringAsync(Key, JsonConvert.SerializeObject(Value));

            var cached = await _distributedCacheProvider.GetAsync<string>(Key);

            cached.Success.ShouldBeTrue();
            cached.Content.ShouldBe(Value);
        }

        [Fact]
        public async Task GetAsync_String_ShouldReturnFailure()
        {
            var cached = await _distributedCacheProvider.GetAsync<string>(Key);

            cached.Success.ShouldBeFalse();
            cached.Content.ShouldBe(default);
        }

        [Fact]
        public void Set_String_ShouldHaveValueInCache()
        {
            _distributedCacheProvider.Set(Key, Value, TimeSpan.FromHours(1));

            var cached = JsonConvert.DeserializeObject<string>(_memoryDistributedCache.GetString(Key));
            cached.ShouldBe(Value);
        }

        [Fact]
        public async Task SetAsync_String_ShouldHaveValueInCache()
        {
            await _distributedCacheProvider.SetAsync(Key, Value, TimeSpan.FromHours(1));

            var cached = JsonConvert.DeserializeObject<string>(await _memoryDistributedCache.GetStringAsync(Key));
            cached.ShouldBe(Value);
        }
    }
}
