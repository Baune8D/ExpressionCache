using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace ExpressionCache.Core;

public sealed class CacheKeyBuilder : ICacheKeyBuilder
{
    public const string NullString = "0x6e756c6c";

    private readonly StringBuilder _builder = new();

    public ICacheKeyBuilder By(object value)
    {
        DateTime? dateTimeValue;
        Guid? guidValue;
        IConvertible convertibleValue;
        IEnumerable enumerableValue;
        ICacheKey cacheKeyValue;

        // Allow null values
        if (value == null)
        {
            _builder.Append(FormatValue(NullString));
        }
        else if ((dateTimeValue = value as DateTime?).HasValue)
        {
            // DateTime is converted by IConvertible, but the default ToString() implementation
            // doesn't have enough granularity to distinguish between unequal DateTimes
            _builder.Append(FormatValue(dateTimeValue.Value.Ticks));
        }
        else if ((guidValue = value as Guid?).HasValue)
        {
            _builder.Append(FormatValue(guidValue.Value));
        }
        else if ((convertibleValue = value as IConvertible) != null)
        {
            _builder.Append(FormatValue(convertibleValue.ToString(CultureInfo.InvariantCulture)));
        }
        else if ((enumerableValue = value as IEnumerable) != null)
        {
            foreach (var element in enumerableValue)
            {
                By(element);
            }
        }
        else if ((cacheKeyValue = value as ICacheKey) != null)
        {
            cacheKeyValue.BuildCacheKey(this);
        }
        else
        {
            throw new ArgumentException(value.GetType() + " cannot be a cache key");
        }

        return this;
    }

    public override string ToString() => _builder.ToString();

    private static string FormatValue<TResult>(TResult value)
    {
        return "{" + value + "}";
    }
}
