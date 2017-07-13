using System;
using System.Threading.Tasks;
using NLog;
using Discord.WebSocket;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Jobs
{
    class SetGameJob : Job
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        public SetGameJob(IServiceProvider services, int interval, Time time) : base(services, interval, time, _Logger) { }
        protected override async Task Run()
        {
            var count = await GlobalConfig.JobConfigDal.GetCount();
            await DiscordClient.SetGameAsync(string.Empty);
            await DiscordClient.SetGameAsync($"A Tale of {count} Gifs");
        }
    }
}
