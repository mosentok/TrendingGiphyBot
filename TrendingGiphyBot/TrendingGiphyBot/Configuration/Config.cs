using Newtonsoft.Json;

namespace TrendingGiphyBot
{
    class Config : IConfig
    {
        [JsonRequired]
        public string DiscordToken { get; set; }
        [JsonRequired]
        public string GiphyToken { get; set; }
        [JsonRequired]
        public string ConnectionString { get; set; }
        [JsonRequired]
        public int MinimumMinutes { get; set; }
        public string WordnikToken { get; set; }
    }
}
