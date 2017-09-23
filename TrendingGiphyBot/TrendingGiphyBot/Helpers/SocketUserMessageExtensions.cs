﻿using System.Collections.Generic;
using System.Linq;
using Discord.WebSocket;

namespace TrendingGiphyBot.Helpers
{
    static class SocketUserMessageExtensions
    {
        internal static bool IsRecognizedModule(this SocketUserMessage message, List<string> modules) => modules.Any(message.Content.ContainsIgnoreCase);
    }
}