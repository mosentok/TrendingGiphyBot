using Discord.WebSocket;
using GiphyDotNet.Manager;
using System;
using System.Threading.Tasks;
using GiphyDotNet.Model.Parameters;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;

namespace TrendingGiphyBot.Configuration
{
    public interface IGlobalConfig : IDisposable
    {
        Config Config { get; set; }
        JobConfigDal JobConfigDal { get; set; }
        UrlCacheDal UrlCacheDal { get; set; }
        UrlHistoryDal UrlHistoryDal { get; set; }
        ChannelConfigDal ChannelConfigDal { get; set; }
        Giphy GiphyClient { get; set; }
        DiscordSocketClient DiscordClient { get; set; }
        JobManager JobManager { get; set; }
        Rating Ratings { get; }
        Task RefreshConfig();
    }
}