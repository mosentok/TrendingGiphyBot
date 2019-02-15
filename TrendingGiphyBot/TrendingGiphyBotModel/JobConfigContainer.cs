using System.Collections.Generic;

namespace TrendingGiphyBotModel
{
    public class JobConfigContainer
    {
        public decimal ChannelId { get; set; }
        public short? Interval { get; set; }
        public string Time { get; set; }
        public string RandomSearchString { get; set; }
        public short? MinQuietHour { get; set; }
        public short? MaxQuietHour { get; set; }
        public string Prefix { get; set; }
        public List<string> Filters { get; set; }
        public JobConfigContainer() { }
        public JobConfigContainer(decimal channelId, short? interval, string time)
        {
            ChannelId = channelId;
            Interval = interval;
            Time = time;
        }
        public JobConfigContainer(JobConfigContainer basedOn, short? interval, string time)
        {
            ChannelId = basedOn.ChannelId;
            Interval = interval;
            Time = time;
            RandomSearchString = basedOn.RandomSearchString;
            MinQuietHour = basedOn.MinQuietHour;
            MaxQuietHour = basedOn.MaxQuietHour;
            //TODO prefix
        }
        public JobConfigContainer(JobConfigContainer basedOn, string randomSearchString)
        {
            ChannelId = basedOn.ChannelId;
            Interval = basedOn.Interval;
            Time = basedOn.Time;
            RandomSearchString = randomSearchString;
            MinQuietHour = basedOn.MinQuietHour;
            MaxQuietHour = basedOn.MaxQuietHour;
            //TODO prefix
        }
        public JobConfigContainer(JobConfigContainer basedOn, short? minQuietHour, short? maxQuietHour)
        {
            ChannelId = basedOn.ChannelId;
            Interval = basedOn.Interval;
            Time = basedOn.Time;
            RandomSearchString = basedOn.RandomSearchString;
            MinQuietHour = minQuietHour;
            MaxQuietHour = maxQuietHour;
            //TODO prefix
        }
        public JobConfigContainer(JobConfigContainer basedOn, List<string> filters)
        {
            ChannelId = basedOn.ChannelId;
            Interval = basedOn.Interval;
            Time = basedOn.Time;
            RandomSearchString = basedOn.RandomSearchString;
            MinQuietHour = basedOn.MinQuietHour;
            MaxQuietHour = basedOn.MaxQuietHour;
            //TODO prefix
            Filters = filters;
        }
    }
}
