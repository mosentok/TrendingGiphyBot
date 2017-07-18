using System;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Helpers
{
    static class TimeExtensions
    {
        public static Time ToTime(this string s) => (Time)Enum.Parse(typeof(Time), s);
    }
}
