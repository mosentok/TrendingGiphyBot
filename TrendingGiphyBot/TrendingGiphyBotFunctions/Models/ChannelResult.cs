using System.Collections.Generic;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Models
{
    public class ChannelResult
    {
        public List<ChannelContainer> ChannelContainers { get; set; }
        public List<UrlHistoryContainer> Errors { get; set; }
        public ChannelResult(List<ChannelContainer> channelContainers, List<UrlHistoryContainer> errors)
        {
            ChannelContainers = channelContainers;
            Errors = errors;
        }
    }
}
