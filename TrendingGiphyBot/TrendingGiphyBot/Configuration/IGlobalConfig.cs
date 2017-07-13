using GiphyDotNet.Manager;
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
    }
}