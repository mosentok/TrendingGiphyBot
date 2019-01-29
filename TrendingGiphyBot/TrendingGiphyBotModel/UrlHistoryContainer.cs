namespace TrendingGiphyBotModel
{
    public class UrlHistoryContainer
    {
        public decimal ChannelId { get; set; }
        public string GifId { get; set; }
        public string Url { get; set; }
        public bool IsTrending { get; set; }
        public UrlHistoryContainer() { }
        public UrlHistoryContainer(decimal channelId, string gifId, string url, bool isTrending)
        {
            ChannelId = channelId;
            GifId = gifId;
            Url = url;
            IsTrending = isTrending;
        }
    }
}
