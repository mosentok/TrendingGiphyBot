using Discord.Commands;
using Discord;
using System.Collections.Generic;
using GiphyDotNet.Manager;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot.CommandContexts
{
    class JobConfigCommandContext : CommandContext
    {
        public List<Job> Jobs { get; set; }
        public JobConfigDal ChannelJobConfigDal { get; set; }
        public Giphy GiphyClient { get; set; }
        public JobConfigCommandContext(IDiscordClient client, IUserMessage msg) : base(client, msg){}
        public JobConfigCommandContext(IDiscordClient client, IUserMessage msg, Giphy giphyClient, List<Job> jobs, JobConfigDal channelJobConfigDal) : this(client, msg)
        {
            GiphyClient = giphyClient;
            Jobs = jobs;
            ChannelJobConfigDal = channelJobConfigDal;
        }
    }
}
