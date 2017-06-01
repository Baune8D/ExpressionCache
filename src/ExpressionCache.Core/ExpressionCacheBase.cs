using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace ExpressionCache.Core
{
    public abstract class ExpressionCacheBase : IExpressionCacheBase
    {
        private readonly IMemoryCache _internalCache;

        protected IExpressionCacheProvider Provider { get; }

        protected ExpressionCacheBase(IExpressionCacheProvider provider)
        {
            _internalCache = new MemoryCache(new MemoryCacheOptions());
            Provider = provider;
        }

        protected ExpressionCacheBase(IMemoryCache internalCache, IExpressionCacheProvider provider)
        {
            _internalCache = internalCache;
            Provider = provider;
        }

        public string GetKey<TResult>(Expression<Func<TResult>> expression)
        {
            return ParseExpression(expression).CacheKey;
        }

        public string GetKey<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            return ParseExpression(expression).CacheKey;
        }

        public TResult InvokeCache<TResult>(Expression<Func<TResult>> expression, TimeSpan expiry, CacheAction cacheAction)
        {
            if (expiry == null) throw new ArgumentNullException(nameof(expiry));
            var expressionResult = ParseExpression(expression);

            if (cacheAction != CacheAction.Overwrite)
            {
                var cachedResult = Provider.Get<TResult>(expressionResult.CacheKey);
                if (cachedResult.Success) return cachedResult.Content;
            }

            var computedValue = (TResult) expressionResult.Method.Invoke(expressionResult.Instance, expressionResult.Arguments);
            if (computedValue != null) Provider.Set(expressionResult.CacheKey, computedValue, expiry);
            return computedValue;
        }

        public async Task<TResult> InvokeCacheAsync<TResult>(Expression<Func<Task<TResult>>> expression, TimeSpan expiry, CacheAction cacheAction)
        {
            if (expiry == null) throw new ArgumentNullException(nameof(expiry));
            var expressionResult = ParseExpression(expression);

            if (cacheAction != CacheAction.Overwrite)
            {
                var cachedResult = await Provider.GetAsync<TResult>(expressionResult.CacheKey);
                if (cachedResult.Success) return cachedResult.Content;
            }

            var computedValue = await (Task<TResult>) expressionResult.Method.Invoke(expressionResult.Instance, expressionResult.Arguments);
            if (computedValue != null) await Provider.SetAsync(expressionResult.CacheKey, computedValue, expiry);
            return computedValue;
        }

        private ExpressionResult ParseExpression(LambdaExpression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            var methodCall = expression.Body as MethodCallExpression;
            if (methodCall == null) throw new ArgumentException("expression: body must be a method call");

            var method = methodCall.Method;

            // We try to cache some of the parsing logic in this function to gain a bit of performance.
            var result = GenerateBaseExpressionResult(expression, methodCall, method);
            result.MethodCall = methodCall;
            result.Method = method;

            // Add function parameter values to arguments array.
            for (var i = 0; i < result.MethodCall.Arguments.Count; ++i)
            {
                result.Arguments[i] = GetValue(result.MethodCall.Arguments[i]);
            }

            // Append the arguments to the base cachekey.
            result.CacheKey = GenerateCacheKey(result.BaseCacheKey, result.Arguments);

            return result;
        }

        private ExpressionResult GenerateBaseExpressionResult(LambdaExpression expression, MethodCallExpression methodCall, MethodInfo method)
        {
            var internalCacheKey = expression.ToString();

            ExpressionResult cached;
            if (_internalCache.TryGetValue(internalCacheKey, out cached))
            {
                // Fetch result from internal cache and clone the result so we dont manipulate it in cache.
                return cached.NewBase();
            }

            var argumentCount = methodCall.Arguments.Count;
            var instance = methodCall.Object != null ? GetValue(methodCall.Object) : null;
            var baseCacheKey = GenerateBaseCacheKey(methodCall, method);

            // Create new cacheable base expression result.
            var baseResult = new ExpressionResult(argumentCount, instance, baseCacheKey);

            // Cache result forever as code should not change by itself at runtime.
            _internalCache.Set(internalCacheKey, baseResult, new MemoryCacheEntryOptions
            {
                Priority = CacheItemPriority.NeverRemove
            });

            // Clone the result so we dont manipulate it in cache.
            return baseResult.NewBase();
        }

        private static string GenerateBaseCacheKey(MethodCallExpression methodCall, MethodInfo methodInfo)
        {
            var keyBuilder = new CacheKeyBuilder()
                .By(methodCall.Object?.Type.Name)
                .By(methodInfo.Name)
                .By(methodInfo.GetGenericArguments());

            return keyBuilder.ToString();
        }

        private static string GenerateCacheKey(string baseKey, object[] arguments)
        {
            var keyBuilder = new CacheKeyBuilder();
            foreach (var argument in arguments) keyBuilder.By(argument);
            return baseKey + keyBuilder;
        }

        private static object GetValue(Expression expression)
        {
            switch (expression.NodeType)
            {
                // We special-case constant and member access because these handle the majority of common cases.
                // E.g. passing a local variable as an argument translates to a field reference on the closure object in expression land.
                case ExpressionType.Constant:
                    return ((ConstantExpression) expression).Value;
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression) expression;
                    var instance = memberExpression.Expression != null ? GetValue(memberExpression.Expression) : null;
                    var field = memberExpression.Member as FieldInfo;
                    return field != null
                        ? field.GetValue(instance)
                        : ((PropertyInfo) memberExpression.Member).GetValue(instance);
                default:
                    // This should always work if the expression CAN be evaluated (E.g. it can't if it references unbound parameters).
                    // However, compilation is slow so the cases above provide valuable performance.
                    var lambda = Expression.Lambda<Func<object>>(Expression.Convert(expression, typeof(object)));
                    return lambda.Compile()();
            }
        }
    }
}
