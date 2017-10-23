using System;
using System.Collections.Generic;
using GiphyDotNet.Manager;
using Newtonsoft.Json;
using System.Configuration;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using GiphyDotNet.Model.Parameters;
using Microsoft.WindowsAzure.Storage;
using NLog;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Configuration
{
    class GlobalConfig : IGlobalConfig
    {
        public Config Config { get; private set; }
        public JobConfigDal JobConfigDal { get; private set; }
        public UrlCacheDal UrlCacheDal { get; private set; }
        public UrlHistoryDal UrlHistoryDal { get; private set; }
        public ChannelConfigDal ChannelConfigDal { get; private set; }
        public Giphy GiphyClient { get; private set; }
        public DiscordSocketClient DiscordClient { get; private set; }
        public JobManager JobManager { get; private set; }
        static readonly Rating _Ratings = Enum.GetValues(typeof(Rating)).OfType<Rating>().Where(s => s != Rating.R).Aggregate((a, b) => a | b);
        public Rating Ratings => _Ratings;
        public List<string> LatestUrls { get; set; }
        public MessageHelper MessageHelper { get; private set; }
        public async Task Initialize()
        {
            await SetConfig();
            JobConfigDal = new JobConfigDal(Config.ConnectionString);
            UrlCacheDal = new UrlCacheDal(Config.ConnectionString);
            UrlHistoryDal = new UrlHistoryDal(Config.ConnectionString);
            ChannelConfigDal = new ChannelConfigDal(Config.ConnectionString);
            GiphyClient = new Giphy(Config.GiphyToken);
            LatestUrls = UrlCacheDal.GetLatestUrls().Result;
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
        }
        public EmbedBuilder BuildEmbedFromConfig(EmbedConfig embedConfig)
        {
            var embedBuilder = new EmbedBuilder()
                .WithDescription(embedConfig.Description)
                .WithImageUrl(embedConfig.ImageUrl);
            if (embedConfig.Author != null)
                embedBuilder.Author = new EmbedAuthorBuilder()
                    .WithName(embedConfig.Author.Name)
                    .WithUrl(embedConfig.Author.Url);
            if (embedConfig.Field != null)
                embedBuilder.Fields.Add(new EmbedFieldBuilder()
                    .WithName(embedConfig.Field.Name)
                    .WithValue(embedConfig.Field.Value));
            return embedBuilder;
        }
        public void Dispose()
        {
            DiscordClient?.LogoutAsync().Wait();
            DiscordClient?.Dispose();
            JobManager?.Dispose();
        }
    }
}
