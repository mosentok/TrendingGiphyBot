using Discord.WebSocket;
using GiphyDotNet.Manager;
using System;
using System.Collections.Generic;
using GiphyDotNet.Model.Parameters;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;

namespace TrendingGiphyBot.Configuration
{
    interface IGlobalConfig : IDisposable
    {
        Config Config { get; set; }
        JobConfigDal JobConfigDal { get; set; }
        UrlCacheDal UrlCacheDal { get; set; }
        UrlHistoryDal UrlHistoryDal { get; set; }
        ChannelConfigDal ChannelConfigDal { get; set; }
        Giphy GiphyClient { get; set; }
        List<Job> Jobs { get; set; }
        DiscordSocketClient DiscordClient { get; set; }
        Rating Ratings { get; }
    }
}