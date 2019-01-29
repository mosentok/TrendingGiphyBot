using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Discord.WebSocket;

namespace TrendingGiphyBot.Extensions
{
    static class SocketUserMessageExtensions
    {
        internal static bool IsRecognizedModule(this SocketMessage message, List<string> modules) => modules.Any(message.Content.ContainsIgnoreCase);
        static bool ContainsIgnoreCase(this string paragraph, string word) =>
            CultureInfo.InvariantCulture.CompareInfo.IndexOf(paragraph, word, CompareOptions.IgnoreCase) >= 0;
    }
}
