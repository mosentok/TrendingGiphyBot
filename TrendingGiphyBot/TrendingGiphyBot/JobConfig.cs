using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrendingGiphyBot
{
    public class JobConfig
    {
        public int Interval { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Time Time { get; set; }
    }
}
