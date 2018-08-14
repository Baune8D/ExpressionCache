using System;
using System.Threading.Tasks;
using ExpressionCache.Redis.Tests.TestHelpers;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using StackExchange.Redis;

namespace ExpressionCache.Redis.Tests
{
    public class DistributedCacheProviderTests : IDisposable
    {
        private readonly RedisFixture _redisFixture;
        private readonly RedisCacheProvider _redisCacheProvider;

        private IDatabase RedisDatabase => _redisFixture.Database;

        private const string Key = "TestKey";
        private const string Value = "TestValue";

        public DistributedCacheProviderTests()
        {
            _redisFixture = new RedisFixture();
            _redisCacheProvider = new RedisCacheProvider(_redisFixture.Options);
        }

        [Fact]
        public void Get_String_ShouldReturnCacheResult()
        {
            RedisDatabase.StringSet(Key, JsonConvert.SerializeObject(Value));

            var cached = _redisCacheProvider.Get<string>(Key);

            cached.Success.ShouldBeTrue();
            cached.Content.ShouldBe(Value);
        }

        [Fact]
        public void Get_String_ShouldReturnFailure()
        {
            var cached = _redisCacheProvider.Get<string>(Key);

            cached.Success.ShouldBeFalse();
            cached.Content.ShouldBe(default);
        }

        [Fact]
        public async Task GetAsync_String_ShouldReturnCacheResult()
        {
            RedisDatabase.StringSet(Key, JsonConvert.SerializeObject(Value));

            var cached = await _redisCacheProvider.GetAsync<string>(Key);

            cached.Success.ShouldBeTrue();
            cached.Content.ShouldBe(Value);
        }

        [Fact]
        public async Task GetAsync_String_ShouldReturnFailure()
        {
            var cached = await _redisCacheProvider.GetAsync<string>(Key);

            cached.Success.ShouldBeFalse();
            cached.Content.ShouldBe(default);
        }

        [Fact]
        public void Set_String_ShouldHaveValueInCache()
        {
            _redisCacheProvider.Set(Key, Value, TimeSpan.FromHours(1));

            var cached = JsonConvert.DeserializeObject<string>(RedisDatabase.StringGet(Key));
            cached.ShouldBe(Value);
        }

        [Fact]
        public async Task SetAsync_String_ShouldHaveValueInCache()
        {
            await _redisCacheProvider.SetAsync(Key, Value, TimeSpan.FromHours(1));

            var cached = JsonConvert.DeserializeObject<string>(RedisDatabase.StringGet(Key));
            cached.ShouldBe(Value);
        }

        public void Dispose()
        {
            _redisFixture.Dispose();
        }
    }
}
