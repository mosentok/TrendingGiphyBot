using System.Globalization;

namespace TrendingGiphyBot.Helpers
{
    static class StringExtensions
    {
        internal static bool ContainsIgnoreCase(this string paragraph, string word) =>
            CultureInfo.InvariantCulture.CompareInfo.IndexOf(paragraph, word, CompareOptions.IgnoreCase) >= 0;
    }
}
