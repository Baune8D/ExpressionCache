namespace ExpressionCache.Core.Testing
{
    public class CacheableObject : ICacheKey
    {
        public CacheableObject(int parameter1, string parameter2)
        {
            Parameter1 = parameter1;
            Parameter2 = parameter2;
        }

        public int Parameter1 { get; set; }

        public string Parameter2 { get; set; }

        public void BuildCacheKey(ICacheKeyBuilder builder)
        {
            builder
                .By(Parameter1)
                .By(Parameter2);
        }
    }
}
