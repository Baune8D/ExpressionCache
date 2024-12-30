namespace ExpressionCache.Core;

public interface ICacheKey
{
    void BuildCacheKey(ICacheKeyBuilder builder);
}
