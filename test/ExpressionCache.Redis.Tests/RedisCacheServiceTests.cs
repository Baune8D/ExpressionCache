using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExpressionCache.Redis.Tests.TestHelpers;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using StackExchange.Redis;

namespace ExpressionCache.Redis.Tests
{
    public class DistributedCacheServiceTests : IClassFixture<TestFunctionsFixture>, IDisposable
    {
        private readonly TestFunctionsFixture _testFunctions;
        private readonly RedisFixture _redisFixture;
        private readonly IRedisCacheService _redisCacheService;

        private IDatabase RedisDatabase => _redisFixture.Database;

        private const string Value = "TestValue";

        public DistributedCacheServiceTests(TestFunctionsFixture testFunctions)
        {
            _testFunctions = testFunctions;
            _redisFixture = new RedisFixture();
            _redisCacheService = new RedisCacheService(_redisFixture.Options);
        }

        [Fact]
        public void Remove_FunctionWithoutParameters_ShouldBeRemoved()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var beforeRemove = RedisDatabase.StringGet(key);

            _redisCacheService.Remove(() => _testFunctions.FunctionWithoutParameters());
            var afterRemove = RedisDatabase.StringGet(key);

            beforeRemove.ShouldNotBeNull();
            afterRemove.ShouldBeNull();
        }

        [Fact]
        public void Remove_FunctionWithoutParametersAsync_ShouldBeRemoved()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var beforeRemove = RedisDatabase.StringGet(key);

            _redisCacheService.Remove(() => _testFunctions.FunctionWithoutParametersAsync());
            var afterRemove = RedisDatabase.StringGet(key);

            beforeRemove.ShouldNotBeNull();
            afterRemove.ShouldBeNull();
        }

        [Fact]
        public async Task RemoveAsync_FunctionWithoutParameters_ShouldBeRemoved()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var beforeRemove = RedisDatabase.StringGet(key);

            await _redisCacheService.RemoveAsync(() => _testFunctions.FunctionWithoutParameters());
            var afterRemove = RedisDatabase.StringGet(key);

            beforeRemove.ShouldNotBeNull();
            afterRemove.ShouldBeNull();
        }

        [Fact]
        public async Task RemoveAsync_FunctionWithoutParametersAsync_ShouldBeRemoved()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var beforeRemove = RedisDatabase.StringGet(key);

            await _redisCacheService.RemoveAsync(() => _testFunctions.FunctionWithoutParametersAsync());
            var afterRemove = RedisDatabase.StringGet(key);

            beforeRemove.ShouldNotBeNull();
            afterRemove.ShouldBeNull();
        }

        [Fact]
        public void Exists_FunctionWithoutParameters_ShouldExist()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            var beforeSet = _redisCacheService.Exists(() => _testFunctions.FunctionWithoutParameters());
            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var afterSet = _redisCacheService.Exists(() => _testFunctions.FunctionWithoutParameters());

            beforeSet.ShouldBeFalse();
            afterSet.ShouldBeTrue();
        }

        [Fact]
        public void Exists_FunctionWithoutParametersAsync_ShouldExist()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            var beforeSet = _redisCacheService.Exists(() => _testFunctions.FunctionWithoutParametersAsync());
            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var afterSet = _redisCacheService.Exists(() => _testFunctions.FunctionWithoutParametersAsync());

            beforeSet.ShouldBeFalse();
            afterSet.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsAsync_FunctionWithoutParameters_ShouldExist()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            var beforeSet = await _redisCacheService.ExistsAsync(() => _testFunctions.FunctionWithoutParameters());
            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var afterSet = await _redisCacheService.ExistsAsync(() => _testFunctions.FunctionWithoutParameters());

            beforeSet.ShouldBeFalse();
            afterSet.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsAsync_FunctionWithoutParametersAsync_ShouldExist()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            var beforeSet = await _redisCacheService.ExistsAsync(() => _testFunctions.FunctionWithoutParametersAsync());
            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var afterSet = await _redisCacheService.ExistsAsync(() => _testFunctions.FunctionWithoutParametersAsync());

            beforeSet.ShouldBeFalse();
            afterSet.ShouldBeTrue();
        }

        [Fact]
        public void Get_FunctionWithoutParameters_ShouldReturnCached()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            var beforeSet = _redisCacheService.Get(() => _testFunctions.FunctionWithoutParameters());
            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var afterSet = _redisCacheService.Get(() => _testFunctions.FunctionWithoutParameters());

            beforeSet.ShouldBeNull();
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public void Get_FunctionWithoutParametersAsync_ShouldReturnCached()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            var beforeSet = _redisCacheService.Get(() => _testFunctions.FunctionWithoutParametersAsync());
            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var afterSet = _redisCacheService.Get(() => _testFunctions.FunctionWithoutParametersAsync());

            beforeSet.ShouldBeNull();
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public async Task GetAsync_FunctionWithoutParameters_ShouldReturnCached()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            var beforeSet = await _redisCacheService.GetAsync(() => _testFunctions.FunctionWithoutParameters());
            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var afterSet = await _redisCacheService.GetAsync(() => _testFunctions.FunctionWithoutParameters());

            beforeSet.ShouldBeNull();
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public async Task GetAsync_FunctionWithoutParametersAsync_ShouldReturnCached()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            var beforeSet = await _redisCacheService.GetAsync(() => _testFunctions.FunctionWithoutParametersAsync());
            RedisDatabase.StringSet(key, JsonConvert.SerializeObject(Value));
            var afterSet = await _redisCacheService.GetAsync(() => _testFunctions.FunctionWithoutParametersAsync());

            beforeSet.ShouldBeNull();
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public async Task GetManyAsync_FunctionWithoutParameters_ShouldReturnCached()
        {
            var expressionList = new List<Expression<Func<string>>>
            {
                () => _testFunctions.FunctionWithoutParameters(),
                () => _testFunctions.OtherFunctionWithoutParameters()
            };

            var key1 = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());
            var key2 = _redisCacheService.GetKey(() => _testFunctions.OtherFunctionWithoutParameters());

            var beforeSet = await _redisCacheService.GetManyAsync(expressionList);

            RedisDatabase.StringSet(key1, JsonConvert.SerializeObject(Value));
            RedisDatabase.StringSet(key2, JsonConvert.SerializeObject(Value + "2"));

            var afterSet = await _redisCacheService.GetManyAsync(expressionList);

            beforeSet.Count.ShouldBe(expressionList.Count);
            beforeSet[0].ShouldBeNull();
            beforeSet[1].ShouldBeNull();
            afterSet.Count.ShouldBe(expressionList.Count);
            afterSet[0].ShouldBe(Value);
            afterSet[1].ShouldBe(Value + "2");
        }

        [Fact]
        public async Task GetManyAsync_FunctionWithoutParametersAsync_ShouldReturnCached()
        {
            var expressionList = new List<Expression<Func<Task<string>>>>
            {
                () => _testFunctions.FunctionWithoutParametersAsync(),
                () => _testFunctions.OtherFunctionWithoutParametersAsync()
            };

            var key1 = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());
            var key2 = _redisCacheService.GetKey(() => _testFunctions.OtherFunctionWithoutParametersAsync());

            var beforeSet = await _redisCacheService.GetManyAsync(expressionList);

            RedisDatabase.StringSet(key1, JsonConvert.SerializeObject(Value));
            RedisDatabase.StringSet(key2, JsonConvert.SerializeObject(Value + "2"));

            var afterSet = await _redisCacheService.GetManyAsync(expressionList);

            beforeSet.Count.ShouldBe(expressionList.Count);
            beforeSet[0].ShouldBeNull();
            beforeSet[1].ShouldBeNull();
            afterSet.Count.ShouldBe(expressionList.Count);
            afterSet[0].ShouldBe(Value);
            afterSet[1].ShouldBe(Value + "2");
        }

        [Fact]
        public void Set_FunctionWithoutParameters_ShouldGetCached()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            _redisCacheService.Set(() => _testFunctions.FunctionWithoutParameters(), Value, TimeSpan.FromHours(1));

            var afterSet = JsonConvert.DeserializeObject<string>(RedisDatabase.StringGet(key));
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public void Set_FunctionWithoutParametersAsync_ShouldGetCached()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            _redisCacheService.Set(() => _testFunctions.FunctionWithoutParametersAsync(), Value, TimeSpan.FromHours(1));

            var afterSet = JsonConvert.DeserializeObject<string>(RedisDatabase.StringGet(key));
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public async Task SetAsync_FunctionWithoutParameters_ShouldGetCached()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            await _redisCacheService.SetAsync(() => _testFunctions.FunctionWithoutParameters(), Value, TimeSpan.FromHours(1));

            var afterSet = JsonConvert.DeserializeObject<string>(RedisDatabase.StringGet(key));
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public async Task SetAsync_FunctionWithoutParametersAsync_ShouldGetCached()
        {
            var key = _redisCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            await _redisCacheService.SetAsync(() => _testFunctions.FunctionWithoutParametersAsync(), Value, TimeSpan.FromHours(1));

            var afterSet = JsonConvert.DeserializeObject<string>(RedisDatabase.StringGet(key));
            afterSet.ShouldBe(Value);
        }

        public void Dispose()
        {
            _redisFixture.Dispose();
        }
    }
}
