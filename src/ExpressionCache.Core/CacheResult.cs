namespace ExpressionCache.Core;

public sealed class CacheResult<TResult>
{
    public bool Success { get; set; }

    // default when Success is false
    public TResult? Content { get; set; }

    public static CacheResult<TResult> Failure()
    {
        return new CacheResult<TResult>
        {
            Success = false,
            Content = default,
        };
    }
}
