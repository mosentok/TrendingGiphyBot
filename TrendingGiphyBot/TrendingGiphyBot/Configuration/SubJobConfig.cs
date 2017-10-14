using Newtonsoft.Json;
using TrendingGiphyBot.Enums;

namespace TrendingGiphyBot.Configuration
{
    public class SubJobConfig
    {
        [JsonRequired]
        public int Interval { get; set; }
        [JsonRequired]
        public Time Time { get; set; }
        public ushort IntervalOffsetSeconds { get; set; }
        public SubJobConfig() { }
        public SubJobConfig(int interval, Time time)
        {
            Interval = interval;
            Time = time;
        }
    }
}
