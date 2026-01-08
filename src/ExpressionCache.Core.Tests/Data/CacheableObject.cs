namespace ExpressionCache.Core.Tests.Data;

public class CacheableObject(int parameter1, string parameter2) : ICacheKey
{
    public int Parameter1 { get; set; } = parameter1;

    public string Parameter2 { get; set; } = parameter2;

    public void BuildCacheKey(ICacheKeyBuilder builder)
    {
        builder
            .By(Parameter1)
            .By(Parameter2);
    }
}
