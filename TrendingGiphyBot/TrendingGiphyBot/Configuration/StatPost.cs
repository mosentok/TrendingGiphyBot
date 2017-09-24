using Newtonsoft.Json;

namespace TrendingGiphyBot.Configuration
{
    public class StatPost
    {
        [JsonRequired]
        public string UrlStringFormat { get; set; }
        [JsonRequired]
        public string Token { get; set; }
    }
}
