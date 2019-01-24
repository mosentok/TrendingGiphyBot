namespace TrendingGiphyBotFunctions.Models
{
    public class UrlHistoryContainer
    {
        public decimal ChannelId { get; set; }
        public string Url { get; set; }
        public bool IsTrending { get; set; }
        public UrlHistoryContainer() { }
        public UrlHistoryContainer(decimal channelId, string url, bool isTrending)
        {
            ChannelId = channelId;
            Url = url;
            IsTrending = isTrending;
        }
    }
}
