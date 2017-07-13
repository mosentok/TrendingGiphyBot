using System;
using System.Threading.Tasks;
using NLog;
using Discord.WebSocket;

namespace TrendingGiphyBot.Jobs
{
    class SetGameJob : Job
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        public SetGameJob(IServiceProvider services, DiscordSocketClient discordClient, int interval, string time) : base(services, discordClient, interval, time, _Logger) { }
        protected override async Task Run()
        {
            var count = await GlobalConfig.JobConfigDal.GetCount();
            await DiscordClient.SetGameAsync(string.Empty);
            await DiscordClient.SetGameAsync($"A Tale of {count} Gifs");
        }
    }
}
