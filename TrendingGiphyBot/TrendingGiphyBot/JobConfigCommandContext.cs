using Discord.Commands;
using Discord;

namespace TrendingGiphyBot
{
    class JobConfigCommandContext : CommandContext
    {
        public Job Job { get; set; }
        public JobConfigCommandContext(IDiscordClient client, IUserMessage msg) : base(client, msg){}
        public JobConfigCommandContext(IDiscordClient client, IUserMessage msg, Job job) : this(client, msg)
        {
            Job = job;
        }
    }
}
