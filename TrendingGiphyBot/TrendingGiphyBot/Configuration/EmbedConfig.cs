using Newtonsoft.Json;

namespace TrendingGiphyBot.Configuration
{
    public class EmbedConfig
    {
        [JsonRequired]
        public string Description { get; set; }
        [JsonRequired]
        public string ImageUrl { get; set; }
        public Author Author { get; set; }
        public Field Field { get; set; }
        public string FooterText { get; set; }
    }
}
