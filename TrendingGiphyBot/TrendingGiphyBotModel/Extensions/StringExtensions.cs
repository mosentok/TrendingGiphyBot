using System;
using System.Collections.Generic;
using System.Linq;

namespace TrendingGiphyBotModel.Extensions
{
    static class StringExtensions
    {
        internal static bool ContainsAnyFilter(this string url, ICollection<string> filters)
        {
            return (from filter in filters
                    where url.Contains(filter, StringComparison.CurrentCultureIgnoreCase)
                    select filter).Any();
        }
    }
}
