using Discord.Commands;
using Discord;
using System.Collections.Generic;
using GiphyDotNet.Manager;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;
using TrendingGiphyBot.Wordnik.Clients;

namespace TrendingGiphyBot.CommandContexts
{
    class JobConfigCommandContext : CommandContext
    {
        public List<Job> Jobs { get; set; }
        public JobConfigDal ChannelJobConfigDal { get; set; }
        public Giphy GiphyClient { get; set; }
        public WordnikClient WordnikClient { get; set; }
        public int MinimumMinutes { get; set; }
        public UrlCacheDal UrlCacheDal { get; set; }
        public JobConfigCommandContext(IDiscordClient client, IUserMessage msg) : base(client, msg) { }
        public JobConfigCommandContext(IDiscordClient client, IUserMessage msg, Giphy giphyClient, List<Job> jobs, JobConfigDal channelJobConfigDal, UrlCacheDal urlCachedal, int minimumMinutes, WordnikClient wordnikClient) : this(client, msg)
        {
            GiphyClient = giphyClient;
            Jobs = jobs;
            ChannelJobConfigDal = channelJobConfigDal;
            UrlCacheDal = urlCachedal;
            MinimumMinutes = minimumMinutes;
            WordnikClient = wordnikClient;
        }
    }
}
