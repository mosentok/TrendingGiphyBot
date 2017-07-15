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
        public int MinimumMinutes { get; set; }
        [JsonRequired]
        public List<LogSeverity> LogSeverities { get; set; }
        public string WordnikBaseAddress { get; set; }
        public string WordnikToken { get; set; }
        public string SpotifyClientId { get; set; }
        public bool UseWordnik => !string.IsNullOrEmpty(WordnikToken);
        public bool UseSpotify => !string.IsNullOrEmpty(SpotifyClientId);
    }
}
