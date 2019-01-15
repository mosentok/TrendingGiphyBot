using System.Collections.Generic;
using Newtonsoft.Json;
using System.Configuration;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using NLog;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Configuration
{
    class GlobalConfig : IGlobalConfig
    {
        public Config Config { get; private set; }
        public EntitiesFactory EntitiesFactory { get; private set; }
        public string GiphyRandomEndpoint { get; set; }
        public string GiphyTrendingEndpoint { get; set; }
        public DiscordSocketClient DiscordClient { get; private set; }
        public JobManager JobManager { get; private set; }
        public MessageHelper MessageHelper { get; private set; }
        public List<int> AllValidMinutes { get; private set; }
        public async Task Initialize()
        {
            await SetConfig();
            EntitiesFactory = new EntitiesFactory(Config.ConnectionString);
            GiphyRandomEndpoint = ConfigurationManager.AppSettings["giphyRandomEndpoint"];
            GiphyTrendingEndpoint = ConfigurationManager.AppSettings["giphyTrendingEndpoint"];
            JobManager = new JobManager(this);
            var configgedLogSeverities = Config.LogSeverities.Aggregate((a, b) => a | b);
            DiscordClient = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = configgedLogSeverities });
        }
        public async Task RefreshConfig()
        {
            await SetConfig();
            JobManager.Ready();
            await DiscordClient.SetGameAsync(Config.PlayingGame);
        }
        async Task SetConfig()
        {
            var connectionString = ConfigurationManager.AppSettings["connectionString"];
            var containerName = ConfigurationManager.AppSettings["containerName"];
            var blobName = ConfigurationManager.AppSettings["blobName"];
            var config = await CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient().GetContainerReference(containerName).GetBlockBlobReference(blobName).DownloadTextAsync();
            LogManager.GetCurrentClassLogger().Trace(config);
            Config = JsonConvert.DeserializeObject<Config>(config);
            MessageHelper = new MessageHelper(Config.FailedReplyDisclaimer);
            var validHoursAsMinutes = Config.ValidHours.Select(s => s * 60);
            AllValidMinutes = Config.ValidMinutes.Concat(validHoursAsMinutes).ToList();
        }
        public void Dispose()
        {
            DiscordClient?.LogoutAsync().Wait();
            DiscordClient?.Dispose();
            JobManager?.Dispose();
        }
    }
}
