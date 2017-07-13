using System;
using System.Threading.Tasks;
using NLog;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Jobs
{
    class SetGameJob : Job
    {
        public SetGameJob(IServiceProvider services, int interval, Time time) : base(services, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected override async Task Run()
        {
            var count = await GlobalConfig.JobConfigDal.GetCount();
            await DiscordClient.SetGameAsync(string.Empty);
            await DiscordClient.SetGameAsync($"A Tale of {count} Gifs");
        }
    }
}
