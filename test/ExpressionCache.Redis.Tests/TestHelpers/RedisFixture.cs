using System;
using StackExchange.Redis;

namespace ExpressionCache.Redis.Tests.TestHelpers
{
    public class RedisFixture : IDisposable
    {
        private RedisInside.Redis _instance;

        public readonly ConfigurationOptions Options;
        public readonly IDatabase Database;

        public RedisFixture()
        {
            _instance = new RedisInside.Redis();
            Options = new ConfigurationOptions
            {
                EndPoints = { _instance.Endpoint }
            };
            Database = ConnectionMultiplexer.Connect(Options).GetDatabase();
        }

        public void Dispose()
        {
            _instance.Dispose();
            _instance = null;
        }
    }
}
