using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Configuration
{
    public interface IGlobalConfig : IDisposable
    {
        Config Config { get; }
        DiscordSocketClient DiscordClient { get; }
        Task Initialize(IFunctionHelper functionHelper);
        Task RefreshConfig();
    }
}