using Discord;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Models
{
    public class ChannelContainer
    {
        public IMessageChannel Channel { get; set; }
        public UrlHistoryContainer HistoryContainer { get; set; }
        public ChannelContainer() { }
        public ChannelContainer(IMessageChannel channel, UrlHistoryContainer history)
        {
            Channel = channel;
            HistoryContainer = history;
        }
    }
}
