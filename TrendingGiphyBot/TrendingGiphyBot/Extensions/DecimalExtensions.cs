using System;

namespace TrendingGiphyBot.Extensions
{
    static class DecimalExtensions
    {
        internal static ulong ToULong(this decimal s) => Convert.ToUInt64(s);
    }
}
