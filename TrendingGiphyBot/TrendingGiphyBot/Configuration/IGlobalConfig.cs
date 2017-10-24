﻿using Discord.WebSocket;
using GiphyDotNet.Manager;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using GiphyDotNet.Model.Parameters;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Helpers;
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
        Task Initialize();
        Task RefreshConfig();
        List<UrlCache> LatestUrls { get; set; }
        EmbedBuilder BuildEmbedFromConfig(EmbedConfig embedConfig);
        MessageHelper MessageHelper { get; }
    }
}