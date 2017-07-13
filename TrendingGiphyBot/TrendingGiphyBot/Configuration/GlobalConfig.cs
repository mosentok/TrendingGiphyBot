using GiphyDotNet.Manager;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;
using TrendingGiphyBot.Wordnik.Clients;

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
        public GlobalConfig()
        {
            var configPath = ConfigurationManager.AppSettings["ConfigPath"];
            var contents = File.ReadAllText(configPath);
            Config = JsonConvert.DeserializeObject<Config>(contents);
            JobConfigDal = new JobConfigDal(Config.ConnectionString);
            UrlCacheDal = new UrlCacheDal(Config.ConnectionString);
            GiphyClient = new Giphy(Config.GiphyToken);
            //TODO base address should be in the config
            WordnikClient = new WordnikClient("http://developer.wordnik.com/v4", Config.WordnikToken);
            Jobs = new List<Job>();
        }
    }
}
