using System.Collections.Generic;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Models
{
    public class GifPostingResult
    {
        public List<UrlHistoryContainer> Errors { get; set; }
        public List<decimal> ChannelsToDelete { get; set; }
        public GifPostingResult(List<UrlHistoryContainer> errors, List<decimal> channelsToDelete)
        {
            Errors = errors;
            ChannelsToDelete = channelsToDelete;
        }
    }
}
