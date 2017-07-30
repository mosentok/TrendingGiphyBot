using System.Collections.Generic;

namespace TrendingGiphyBot.Helpers
{
    static class ListExtensions
    {
        internal static string Join(this List<int> ints, string separator) => string.Join(separator, ints);
    }
}
