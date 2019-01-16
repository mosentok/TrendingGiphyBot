namespace TrendingGiphyBotFunctions.Models
{
    public partial class JobConfig
    {
        public decimal ChannelId { get; set; }
        public int Interval { get; set; }
        public string Time { get; set; }
        public bool RandomIsOn { get; set; }
        public string RandomSearchString { get; set; }
        public short? MinQuietHour { get; set; }
        public short? MaxQuietHour { get; set; }
        public int IntervalMinutes { get; set; }
        public string Prefix { get; set; }

        public virtual Time TimeNavigation { get; set; }
    }
}
