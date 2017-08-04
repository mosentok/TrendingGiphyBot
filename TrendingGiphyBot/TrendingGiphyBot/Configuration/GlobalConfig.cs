using System;
using GiphyDotNet.Manager;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using GiphyDotNet.Model.Parameters;
using Microsoft.WindowsAzure.Storage;
using NLog;

namespace TrendingGiphyBot.Configuration
{
    class GlobalConfig : IGlobalConfig
    {
        public Config Config { get; set; }
        public JobConfigDal JobConfigDal { get; set; }
        public UrlCacheDal UrlCacheDal { get; set; }
        public UrlHistoryDal UrlHistoryDal { get; set; }
        public ChannelConfigDal ChannelConfigDal { get; set; }
        public Giphy GiphyClient { get; set; }
        public List<Job> Jobs { get; set; }
        public DiscordSocketClient DiscordClient { get; set; }
        static readonly Rating _Ratings = Enum.GetValues(typeof(Rating)).OfType<Rating>().Where(s => s != Rating.R).Aggregate((a, b) => a | b);
        public Rating Ratings => _Ratings;
        public GlobalConfig()
        {
            RefreshConfig().Wait();
            JobConfigDal = new JobConfigDal(Config.ConnectionString);
            UrlCacheDal = new UrlCacheDal(Config.ConnectionString);
            UrlHistoryDal = new UrlHistoryDal(Config.ConnectionString);
            ChannelConfigDal = new ChannelConfigDal(Config.ConnectionString);
            GiphyClient = new Giphy(Config.GiphyToken);
            Jobs = new List<Job>();
            var configgedLogSeverities = Config.LogSeverities.Aggregate((a, b) => a | b);
            DiscordClient = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = configgedLogSeverities });
        }
        public async Task RefreshConfig()
        {
            var connectionString = ConfigurationManager.AppSettings["connectionString"];
            var containerName = ConfigurationManager.AppSettings["containerName"];
            var blobName = ConfigurationManager.AppSettings["blobName"];
            var config = await CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient().GetContainerReference(containerName).GetBlockBlobReference(blobName).DownloadTextAsync();
            LogManager.GetCurrentClassLogger().Trace(config);
            Config = JsonConvert.DeserializeObject<Config>(config);
        }
        public void Dispose()
        {
            DiscordClient?.LogoutAsync().Wait();
            DiscordClient?.Dispose();
            Jobs?.ForEach(s => s?.Dispose());
        }
    }
}
