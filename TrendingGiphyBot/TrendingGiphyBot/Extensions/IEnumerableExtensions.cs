using System.Collections.Generic;

namespace TrendingGiphyBot.Extensions
{
    static class IEnumerableExtensions
    {
        internal static string FlattenWith<T>(this IEnumerable<T> values, string separator) => string.Join(separator, values);
    }
}
