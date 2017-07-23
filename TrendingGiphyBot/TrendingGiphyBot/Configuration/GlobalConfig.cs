using GiphyDotNet.Manager;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;
using Discord.WebSocket;
using System.Linq;
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
            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (var entities = new TrendingGiphyBotEntities(connectionString))
            {
                var config = entities.BotConfigs.Single(s => s.Key == "TrendingGiphyBot").Value;
                LogManager.GetCurrentClassLogger().Trace(config);
                Config = JsonConvert.DeserializeObject<Config>(config);
            }
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
