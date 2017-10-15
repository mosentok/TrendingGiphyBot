﻿using System;
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

namespace TrendingGiphyBot.Configuration
{
    class GlobalConfig : IGlobalConfig
    {
        public Config Config { get; private set; }
        public JobConfigDal JobConfigDal { get; }
        public UrlCacheDal UrlCacheDal { get; }
        public UrlHistoryDal UrlHistoryDal { get; }
        public ChannelConfigDal ChannelConfigDal { get; }
        public Giphy GiphyClient { get; }
        public DiscordSocketClient DiscordClient { get; }
        public JobManager JobManager { get; }
        static readonly Rating _Ratings = Enum.GetValues(typeof(Rating)).OfType<Rating>().Where(s => s != Rating.R).Aggregate((a, b) => a | b);
        public Rating Ratings => _Ratings;
        public Lazy<Embed> WelcomeMessagEmbed { get; private set; }
        public Lazy<Embed> HelpMessagEmbed { get; private set; }
        public List<string> LatestUrls { get; set; }
        public GlobalConfig()
        {
            SetConfig().Wait();
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
            WelcomeMessagEmbed = new Lazy<Embed>(GetWelcomeMessageEmbed);
            HelpMessagEmbed = new Lazy<Embed>(GetHelpMessageEmbed);
        }
        Embed GetWelcomeMessageEmbed()
        {
            var welcomeMessage = Config.WelcomeMessage;
            var embedBuilder = new EmbedBuilder()
                .WithDescription(welcomeMessage.Description)
                .WithImageUrl(welcomeMessage.ImageUrl);
            if (welcomeMessage.Author != null)
                embedBuilder.Author = new EmbedAuthorBuilder()
                    .WithName(welcomeMessage.Author.Name)
                    .WithUrl(welcomeMessage.Author.Url);
            if (welcomeMessage.Field != null)
                embedBuilder.Fields.Add(new EmbedFieldBuilder()
                    .WithName(welcomeMessage.Field.Name)
                    .WithValue(welcomeMessage.Field.Value));
            return embedBuilder.Build();
        }
        Embed GetHelpMessageEmbed()
        {
            var helpMessage = Config.HelpMessage;
            return new EmbedBuilder()
                .WithDescription(helpMessage.Description)
                .WithImageUrl(helpMessage.ImageUrl)
                .Build();
        }
        public void Dispose()
        {
            DiscordClient?.LogoutAsync().Wait();
            DiscordClient?.Dispose();
            JobManager?.Dispose();
        }
    }
}
