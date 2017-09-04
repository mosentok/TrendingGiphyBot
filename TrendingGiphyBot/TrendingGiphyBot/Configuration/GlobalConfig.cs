﻿using System;
using GiphyDotNet.Manager;
using Newtonsoft.Json;
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
        public Config Config { get; }
        public JobConfigDal JobConfigDal { get; }
        public UrlCacheDal UrlCacheDal { get; }
        public UrlHistoryDal UrlHistoryDal { get; }
        public ChannelConfigDal ChannelConfigDal { get; }
        public Giphy GiphyClient { get; }
        public DiscordSocketClient DiscordClient { get; }
        public JobManager JobManager { get; }
        static readonly Rating _Ratings = Enum.GetValues(typeof(Rating)).OfType<Rating>().Where(s => s != Rating.R).Aggregate((a, b) => a | b);
        public Rating Ratings => _Ratings;
        public GlobalConfig()
        {
            SetConfig().Wait();
            JobConfigDal = new JobConfigDal(Config.ConnectionString);
            UrlCacheDal = new UrlCacheDal(Config.ConnectionString);
            UrlHistoryDal = new UrlHistoryDal(Config.ConnectionString);
            ChannelConfigDal = new ChannelConfigDal(Config.ConnectionString);
            GiphyClient = new Giphy(Config.GiphyToken);
            JobManager = new JobManager(this);
            var configgedLogSeverities = Config.LogSeverities.Aggregate((a, b) => a | b);
            DiscordClient = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = configgedLogSeverities });
        }
        public async Task RefreshConfig()
        {
            await SetConfig();
            JobManager.Ready();
        }
        async Task SetConfig()
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
            JobManager?.Dispose();
        }
    }
}
