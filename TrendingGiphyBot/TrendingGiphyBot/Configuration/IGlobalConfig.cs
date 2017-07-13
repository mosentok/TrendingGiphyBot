using Discord.WebSocket;
using GiphyDotNet.Manager;
using SpotifyAPI.Web;
using System.Collections.Generic;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;
using TrendingGiphyBot.Wordnik.Clients;

namespace TrendingGiphyBot.Configuration
{
    interface IGlobalConfig
    {
        Config Config { get; set; }
        JobConfigDal JobConfigDal { get; set; }
        UrlCacheDal UrlCacheDal { get; set; }
        Giphy GiphyClient { get; set; }
        WordnikClient WordnikClient { get; set; }
        List<Job> Jobs { get; set; }
        DiscordSocketClient DiscordClient { get; set; }
        SpotifyWebAPI SpotifyClient { get; set; }
    }
}