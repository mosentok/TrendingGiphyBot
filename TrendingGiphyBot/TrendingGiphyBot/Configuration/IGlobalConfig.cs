using Discord.WebSocket;
using GiphyDotNet.Manager;
using System;
using System.Threading.Tasks;
using Discord;
using GiphyDotNet.Model.Parameters;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;

namespace TrendingGiphyBot.Configuration
{
    public interface IGlobalConfig : IDisposable
    {
        Config Config { get; }
        JobConfigDal JobConfigDal { get; }
        UrlCacheDal UrlCacheDal { get; }
        UrlHistoryDal UrlHistoryDal { get; }
        ChannelConfigDal ChannelConfigDal { get; }
        Giphy GiphyClient { get; }
        DiscordSocketClient DiscordClient { get; }
        JobManager JobManager { get; }
        Rating Ratings { get; }
        Task RefreshConfig();
        Lazy<Embed> WelcomeMessagEmbed { get; }
        Lazy<Embed> HelpMessagEmbed { get; }
    }
}