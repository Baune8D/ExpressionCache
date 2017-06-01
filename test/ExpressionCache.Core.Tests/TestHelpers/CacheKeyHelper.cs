namespace ExpressionCache.Core.Tests.TestHelpers
{
    public static class CacheKeyHelper
    {
        public static string Format<TResult>(TResult value)
        {
            return "{" + value + "}";
        }
    }
}
