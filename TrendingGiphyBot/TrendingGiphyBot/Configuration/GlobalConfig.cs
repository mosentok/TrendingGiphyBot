using GiphyDotNet.Manager;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;
using TrendingGiphyBot.Wordnik.Clients;
using Discord.WebSocket;
using System;
using Discord;
using System.Linq;

namespace TrendingGiphyBot.Configuration
{
    class GlobalConfig : IGlobalConfig
    {
        public Config Config { get; set; }
        public JobConfigDal JobConfigDal { get; set; }
        public UrlCacheDal UrlCacheDal { get; set; }
        public Giphy GiphyClient { get; set; }
        public WordnikClient WordnikClient { get; set; }
        public List<Job> Jobs { get; set; }
        public DiscordSocketClient DiscordClient { get; set; }
        public GlobalConfig()
        {
            var configPath = ConfigurationManager.AppSettings["ConfigPath"];
            var contents = File.ReadAllText(configPath);
            Config = JsonConvert.DeserializeObject<Config>(contents);
            JobConfigDal = new JobConfigDal(Config.ConnectionString);
            UrlCacheDal = new UrlCacheDal(Config.ConnectionString);
            GiphyClient = new Giphy(Config.GiphyToken);
            WordnikClient = new WordnikClient(Config.WordnikBaseAddress, Config.WordnikToken);
            Jobs = new List<Job>();
            var allLogSeverities = Enum.GetValues(typeof(LogSeverity)).OfType<LogSeverity>().Aggregate((a, b) => a | b);
            DiscordClient = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = allLogSeverities });
        }
    }
}
