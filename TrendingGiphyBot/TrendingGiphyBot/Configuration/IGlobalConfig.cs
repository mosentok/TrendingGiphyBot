using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Helpers;
using TrendingGiphyBot.Jobs;

namespace TrendingGiphyBot.Configuration
{
    public interface IGlobalConfig : IDisposable
    {
        Config Config { get; }
        EntitiesFactory EntitiesFactory { get; }
        string GiphyRandomEndpoint { get; }
        string GiphyTrendingEndpoint { get; }
        DiscordSocketClient DiscordClient { get; }
        JobManager JobManager { get; }
        Task Initialize();
        Task RefreshConfig();
        MessageHelper MessageHelper { get; }
        List<int> AllValidMinutes { get; }
    }
}