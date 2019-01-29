using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Discord.WebSocket;

namespace TrendingGiphyBotCore.Extensions
{
    public static class SocketUserMessageExtensions
    {
        public static bool IsRecognizedModule(this SocketMessage message, List<string> modules) => modules.Any(message.Content.ContainsIgnoreCase);
        static bool ContainsIgnoreCase(this string paragraph, string word) =>
            CultureInfo.InvariantCulture.CompareInfo.IndexOf(paragraph, word, CompareOptions.IgnoreCase) >= 0;
    }
}
