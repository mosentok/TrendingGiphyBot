using Discord;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrendingGiphyBot.Configuration
{
    class Config
    {
        [JsonRequired]
        public string DiscordToken { get; set; }
        [JsonRequired]
        public string GiphyToken { get; set; }
        [JsonRequired]
        public string ConnectionString { get; set; }
        [JsonRequired]
        public SubJobConfig RefreshImageJobConfig { get; set; }
        [JsonRequired]
        public SubJobConfig MinJobConfig { get; set; }
        [JsonRequired]
        public SubJobConfig MaxJobConfig { get; set; }
        [JsonRequired]
        public List<int> ValidSeconds { get; set; }
        [JsonRequired]
        public List<int> ValidMinutes { get; set; }
        [JsonRequired]
        public List<int> ValidHours { get; set; }
        [JsonRequired]
        public List<LogSeverity> LogSeverities { get; set; }
        [JsonRequired]
        public string DefaultPrefix { get; set; }
        public string WordnikBaseAddress { get; set; }
        public string WordnikToken { get; set; }
        public string SpotifyClientId { get; set; }
        [JsonRequired]
        public string GitHubUrl { get; set; }
        [JsonRequired]
        public string PlayingGame { get; set; }
    }
}
