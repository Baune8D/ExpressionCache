namespace ExpressionCache.Core
{
    public interface ICacheKeyBuilder
    {
        ICacheKeyBuilder By(object value);
    }
}
