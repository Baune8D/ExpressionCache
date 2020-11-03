using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExpressionCache.Distributed.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Microsoft.Extensions.Options;

namespace ExpressionCache.Distributed.Tests
{
    public class DistributedCacheServiceTests : IClassFixture<TestFunctionsFixture>
    {
        private readonly TestFunctionsFixture _testFunctions;

        private readonly MemoryDistributedCache _memoryDistributedCache;
        private readonly IDistributedCacheService _distributedCacheService;

        private const string Value = "TestValue";

        public DistributedCacheServiceTests(TestFunctionsFixture testFunctions)
        {
            _testFunctions = testFunctions;

            var options = new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
            _memoryDistributedCache = new MemoryDistributedCache(options);
            _distributedCacheService = new DistributedCacheService(_memoryDistributedCache);
        }

        [Fact]
        public void Remove_FunctionWithoutParameters_ShouldBeRemoved()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var beforeRemove = _memoryDistributedCache.GetString(key);

            _distributedCacheService.Remove(() => _testFunctions.FunctionWithoutParameters());
            var afterRemove = _memoryDistributedCache.GetString(key);

            beforeRemove.ShouldNotBeNull();
            afterRemove.ShouldBeNull();
        }

        [Fact]
        public void Remove_FunctionWithoutParametersAsync_ShouldBeRemoved()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var beforeRemove = _memoryDistributedCache.GetString(key);

            _distributedCacheService.Remove(() => _testFunctions.FunctionWithoutParametersAsync());
            var afterRemove = _memoryDistributedCache.GetString(key);

            beforeRemove.ShouldNotBeNull();
            afterRemove.ShouldBeNull();
        }

        [Fact]
        public async Task RemoveAsync_FunctionWithoutParameters_ShouldBeRemoved()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var beforeRemove = _memoryDistributedCache.GetString(key);

            await _distributedCacheService.RemoveAsync(() => _testFunctions.FunctionWithoutParameters());
            var afterRemove = _memoryDistributedCache.GetString(key);

            beforeRemove.ShouldNotBeNull();
            afterRemove.ShouldBeNull();
        }

        [Fact]
        public async Task RemoveAsync_FunctionWithoutParametersAsync_ShouldBeRemoved()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var beforeRemove = _memoryDistributedCache.GetString(key);

            await _distributedCacheService.RemoveAsync(() => _testFunctions.FunctionWithoutParametersAsync());
            var afterRemove = _memoryDistributedCache.GetString(key);

            beforeRemove.ShouldNotBeNull();
            afterRemove.ShouldBeNull();
        }

        [Fact]
        public void Exists_FunctionWithoutParameters_ShouldExist()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            var beforeSet = _distributedCacheService.Exists(() => _testFunctions.FunctionWithoutParameters());
            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var afterSet = _distributedCacheService.Exists(() => _testFunctions.FunctionWithoutParameters());

            beforeSet.ShouldBeFalse();
            afterSet.ShouldBeTrue();
        }

        [Fact]
        public void Exists_FunctionWithoutParametersAsync_ShouldExist()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            var beforeSet = _distributedCacheService.Exists(() => _testFunctions.FunctionWithoutParametersAsync());
            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var afterSet = _distributedCacheService.Exists(() => _testFunctions.FunctionWithoutParametersAsync());

            beforeSet.ShouldBeFalse();
            afterSet.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsAsync_FunctionWithoutParameters_ShouldExist()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            var beforeSet = await _distributedCacheService.ExistsAsync(() => _testFunctions.FunctionWithoutParameters());
            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var afterSet = await _distributedCacheService.ExistsAsync(() => _testFunctions.FunctionWithoutParameters());

            beforeSet.ShouldBeFalse();
            afterSet.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsAsync_FunctionWithoutParametersAsync_ShouldExist()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            var beforeSet = await _distributedCacheService.ExistsAsync(() => _testFunctions.FunctionWithoutParametersAsync());
            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var afterSet = await _distributedCacheService.ExistsAsync(() => _testFunctions.FunctionWithoutParametersAsync());

            beforeSet.ShouldBeFalse();
            afterSet.ShouldBeTrue();
        }

        [Fact]
        public void Get_FunctionWithoutParameters_ShouldReturnCached()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            var beforeSet = _distributedCacheService.Get(() => _testFunctions.FunctionWithoutParameters());
            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var afterSet = _distributedCacheService.Get(() => _testFunctions.FunctionWithoutParameters());

            beforeSet.ShouldBeNull();
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public void Get_FunctionWithoutParametersAsync_ShouldReturnCached()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            var beforeSet = _distributedCacheService.Get(() => _testFunctions.FunctionWithoutParametersAsync());
            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var afterSet = _distributedCacheService.Get(() => _testFunctions.FunctionWithoutParametersAsync());

            beforeSet.ShouldBeNull();
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public async Task GetAsync_FunctionWithoutParameters_ShouldReturnCached()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            var beforeSet = await _distributedCacheService.GetAsync(() => _testFunctions.FunctionWithoutParameters());
            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var afterSet = await _distributedCacheService.GetAsync(() => _testFunctions.FunctionWithoutParameters());

            beforeSet.ShouldBeNull();
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public async Task GetAsync_FunctionWithoutParametersAsync_ShouldReturnCached()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            var beforeSet = await _distributedCacheService.GetAsync(() => _testFunctions.FunctionWithoutParametersAsync());
            _memoryDistributedCache.SetString(key, JsonConvert.SerializeObject(Value));
            var afterSet = await _distributedCacheService.GetAsync(() => _testFunctions.FunctionWithoutParametersAsync());

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

            var key1 = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());
            var key2 = _distributedCacheService.GetKey(() => _testFunctions.OtherFunctionWithoutParameters());

            var beforeSet = await _distributedCacheService.GetManyAsync(expressionList);

            _memoryDistributedCache.SetString(key1, JsonConvert.SerializeObject(Value));
            _memoryDistributedCache.SetString(key2, JsonConvert.SerializeObject(Value + "2"));

            var afterSet = await _distributedCacheService.GetManyAsync(expressionList);

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

            var key1 = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());
            var key2 = _distributedCacheService.GetKey(() => _testFunctions.OtherFunctionWithoutParametersAsync());

            var beforeSet = await _distributedCacheService.GetManyAsync(expressionList);

            _memoryDistributedCache.SetString(key1, JsonConvert.SerializeObject(Value));
            _memoryDistributedCache.SetString(key2, JsonConvert.SerializeObject(Value + "2"));

            var afterSet = await _distributedCacheService.GetManyAsync(expressionList);

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
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            _distributedCacheService.Set(() => _testFunctions.FunctionWithoutParameters(), Value, TimeSpan.FromHours(1));

            var afterSet = JsonConvert.DeserializeObject<string>(_memoryDistributedCache.GetString(key));
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public void Set_FunctionWithoutParametersAsync_ShouldGetCached()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            _distributedCacheService.Set(() => _testFunctions.FunctionWithoutParametersAsync(), Value, TimeSpan.FromHours(1));

            var afterSet = JsonConvert.DeserializeObject<string>(_memoryDistributedCache.GetString(key));
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public async Task SetAsync_FunctionWithoutParameters_ShouldGetCached()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParameters());

            await _distributedCacheService.SetAsync(() => _testFunctions.FunctionWithoutParameters(), Value, TimeSpan.FromHours(1));

            var afterSet = JsonConvert.DeserializeObject<string>(_memoryDistributedCache.GetString(key));
            afterSet.ShouldBe(Value);
        }

        [Fact]
        public async Task SetAsync_FunctionWithoutParametersAsync_ShouldGetCached()
        {
            var key = _distributedCacheService.GetKey(() => _testFunctions.FunctionWithoutParametersAsync());

            await _distributedCacheService.SetAsync(() => _testFunctions.FunctionWithoutParametersAsync(), Value, TimeSpan.FromHours(1));

            var afterSet = JsonConvert.DeserializeObject<string>(_memoryDistributedCache.GetString(key));
            afterSet.ShouldBe(Value);
        }
    }
}
