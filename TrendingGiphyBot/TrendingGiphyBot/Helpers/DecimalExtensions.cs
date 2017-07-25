using System;

namespace TrendingGiphyBot.Helpers
{
    static class DecimalExtensions
    {
        internal static ulong ToULong(this decimal s) => Convert.ToUInt64(s);
    }
}
