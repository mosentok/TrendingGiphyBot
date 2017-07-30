using GiphyDotNet.Manager;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;
using Discord.WebSocket;
using System.Linq;
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
        public GlobalConfig()
        {
            var connectionString = ConfigurationManager.AppSettings["connectionString"];
            var shareName = ConfigurationManager.AppSettings["shareName"];
            var directoryName = ConfigurationManager.AppSettings["directoryName"];
            var fileName = ConfigurationManager.AppSettings["fileName"];
            var config = CloudStorageAccount
                .Parse(connectionString).CreateCloudFileClient().GetShareReference(shareName)
                .GetRootDirectoryReference().GetDirectoryReference(directoryName).GetFileReference(fileName)
                .DownloadText();
            LogManager.GetCurrentClassLogger().Trace(config);
            Config = JsonConvert.DeserializeObject<Config>(config);
            JobConfigDal = new JobConfigDal(Config.ConnectionString);
            UrlCacheDal = new UrlCacheDal(Config.ConnectionString);
            UrlHistoryDal = new UrlHistoryDal(Config.ConnectionString);
            ChannelConfigDal = new ChannelConfigDal(Config.ConnectionString);
            GiphyClient = new Giphy(Config.GiphyToken);
            Jobs = new List<Job>();
            var configgedLogSeverities = Config.LogSeverities.Aggregate((a, b) => a | b);
            DiscordClient = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = configgedLogSeverities });
        }
        public void Dispose()
        {
            DiscordClient?.LogoutAsync().Wait();
            DiscordClient?.Dispose();
            Jobs?.ForEach(s => s?.Dispose());
        }
    }
}
