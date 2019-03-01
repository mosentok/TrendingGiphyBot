using TrendingGiphyBotCore.Enums;

namespace TrendingGiphyBotCore.Configuration
{
    //TODO replace this with TimeSpan
    public class SubJobConfig
    {
        public short Interval { get; set; }
        public Time Time { get; set; }
        public SubJobConfig() { }
        public SubJobConfig(short interval, Time time)
        {
            Interval = interval;
            Time = time;
        }
    }
}
