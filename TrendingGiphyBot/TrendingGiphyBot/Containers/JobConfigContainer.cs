namespace TrendingGiphyBot.Containers
{
    public class JobConfigContainer
    {
        public decimal ChannelId { get; set; }
        public int Interval { get; set; }
        public string Time { get; set; }
        public bool RandomIsOn { get; set; }
        public string RandomSearchString { get; set; }
        public short? MinQuietHour { get; set; }
        public short? MaxQuietHour { get; set; }
        public JobConfigContainer() { }
        public JobConfigContainer(decimal channelId, int interval, string time)
        {
            ChannelId = channelId;
            Interval = interval;
            Time = time;
        }
        public JobConfigContainer(JobConfigContainer basedOn, int interval, string time)
        {
            ChannelId = basedOn.ChannelId;
            Interval = interval;
            Time = time;
            RandomIsOn = basedOn.RandomIsOn;
            RandomSearchString = basedOn.RandomSearchString;
            MinQuietHour = basedOn.MinQuietHour;
            MaxQuietHour = basedOn.MaxQuietHour;
        }
        public JobConfigContainer(JobConfigContainer basedOn, bool randomIsOn, string randomSearchString)
        {
            ChannelId = basedOn.ChannelId;
            Interval = basedOn.Interval;
            Time = basedOn.Time;
            RandomIsOn = randomIsOn;
            RandomSearchString = randomSearchString;
            MinQuietHour = basedOn.MinQuietHour;
            MaxQuietHour = basedOn.MaxQuietHour;
        }
        public JobConfigContainer(JobConfigContainer basedOn, short? minQuietHour, short? maxQuietHour)
        {
            ChannelId = basedOn.ChannelId;
            Interval = basedOn.Interval;
            Time = basedOn.Time;
            RandomIsOn = basedOn.RandomIsOn;
            RandomSearchString = basedOn.RandomSearchString;
            MinQuietHour = minQuietHour;
            MaxQuietHour = maxQuietHour;
        }
    }
}
