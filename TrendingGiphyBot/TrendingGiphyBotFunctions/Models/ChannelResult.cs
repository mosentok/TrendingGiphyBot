using System.Collections.Generic;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Models
{
    public class ChannelResult
    {
        public List<ChannelContainer> ChannelContainers { get; set; }
        public List<UrlHistoryContainer> Errors { get; set; }
        public List<decimal> ChannelsToDelete { get; set; }
        public ChannelResult(List<ChannelContainer> channelContainers, List<UrlHistoryContainer> errors, List<decimal> channelsToDelete)
        {
            ChannelContainers = channelContainers;
            Errors = errors;
            ChannelsToDelete = channelsToDelete;
        }
    }
}
