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
using System.Linq;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Auth;

namespace TrendingGiphyBot.Configuration
{
    class GlobalConfig : IGlobalConfig
    {
        public Config Config { get; set; }
        public JobConfigDal JobConfigDal { get; set; }
        public UrlCacheDal UrlCacheDal { get; set; }
        public UrlHistoryDal UrlHistoryDal { get; set; }
        public Giphy GiphyClient { get; set; }
        public WordnikClient WordnikClient { get; set; }
        public List<Job> Jobs { get; set; }
        public DiscordSocketClient DiscordClient { get; set; }
        public SpotifyWebAPI SpotifyClient { get; set; }
        public GlobalConfig()
        {
            var configPath = ConfigurationManager.AppSettings["ConfigPath"];
            var contents = File.ReadAllText(configPath);
            Config = JsonConvert.DeserializeObject<Config>(contents);
            JobConfigDal = new JobConfigDal(Config.ConnectionString);
            UrlCacheDal = new UrlCacheDal(Config.ConnectionString);
            UrlHistoryDal = new UrlHistoryDal(Config.ConnectionString);
            GiphyClient = new Giphy(Config.GiphyToken);
            if (Config.UseWordnik)
                WordnikClient = new WordnikClient(Config.WordnikBaseAddress, Config.WordnikToken);
            Jobs = new List<Job>();
            var configgedLogSeverities = Config.LogSeverities.Aggregate((a, b) => a | b);
            DiscordClient = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = configgedLogSeverities });
            if (Config.UseSpotify)
            {
                var webApiFactory = new WebAPIFactory("http://localhost", 8000, Config.SpotifyClientId, Scope.UserReadPlaybackState, TimeSpan.FromSeconds(20));
                SpotifyClient = webApiFactory.GetWebApi().Result;
            }
        }
        public void Dispose()
        {
            DiscordClient?.LogoutAsync().Wait();
            DiscordClient?.Dispose();
            WordnikClient?.Dispose();
            SpotifyClient?.Dispose();
            Jobs?.ForEach(s => s?.Dispose());
        }
    }
}
