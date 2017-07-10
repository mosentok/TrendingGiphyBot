using System.Threading.Tasks;
using Discord.WebSocket;
using GiphyDotNet.Manager;
using NLog;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot.Jobs
{
    class SetGameJob : Job
    {
        static readonly ILogger _Logger = LogManager.GetCurrentClassLogger();
        readonly JobConfigDal _JobConfigDal;
        internal SetGameJob(Giphy giphyClient, DiscordSocketClient discordClient, int interval, string time, JobConfigDal jobConfigdal) : base(giphyClient, discordClient, interval, time, _Logger)
        {
            _JobConfigDal = jobConfigdal;
        }
        protected override async Task Run()
        {
            var count = await _JobConfigDal.GetCount();
            await DiscordClient.SetGameAsync(string.Empty);
            await DiscordClient.SetGameAsync($"A Tale of {count} Gifs");
        }
    }
}
