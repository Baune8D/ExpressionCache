using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionCache.Core
{
    public sealed class ExpressionResult
    {
        private int ArgumentCount { get; }
        public object Instance { get; }
        public string BaseCacheKey { get; }

        public object[] Arguments;
        public MethodInfo Method;
        public MethodCallExpression MethodCall;
        public string CacheKey;

        public ExpressionResult(int argumentCount, object instance, string baseCacheKey)
        {
            ArgumentCount = argumentCount;

            Instance = instance;
            BaseCacheKey = baseCacheKey;

            Arguments = new object[ArgumentCount]; // Values should not be cached
            Method = null; // Should not be cached
            MethodCall = null; // Should not be cached
            CacheKey = null; // Should not be cached
        }

        public ExpressionResult NewBase()
        {
            return new ExpressionResult(ArgumentCount, Instance, BaseCacheKey);
        }
    }
}
