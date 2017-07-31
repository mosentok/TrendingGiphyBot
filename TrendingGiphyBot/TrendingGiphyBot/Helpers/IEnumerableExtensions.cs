using System.Collections.Generic;

namespace TrendingGiphyBot.Helpers
{
    static class IEnumerableExtensions
    {
        internal static string Join(this IEnumerable<int> ints, string separator) => string.Join(separator, ints);
        internal static string Join(this IEnumerable<string> ints, string separator) => string.Join(separator, ints);
    }
}
