using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrendingGiphyBot.Helpers
{
    static class IEnumerableExtensions
    {
        internal static string FlattenWith<T>(this IEnumerable<T> values, string separator) => string.Join(separator, values);
        internal static async Task<List<TSource>> WhereAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> predicate)
        {
            var results = new List<TSource>();
            foreach (var element in source)
                if (await predicate(element))
                    results.Add(element);
            return results;
        }
        internal static async Task<TSource> FirstOrDefaultAsync<TSource>(this IEnumerable<TSource> source, Func<TSource, Task<bool>> predicate)
        {
            foreach (var element in source)
                if (await predicate(element))
                    return element;
            return default;
        }
    }
}
