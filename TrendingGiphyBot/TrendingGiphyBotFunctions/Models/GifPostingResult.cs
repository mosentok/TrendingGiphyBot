using System.Collections.Generic;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Models
{
    public class GifPostingResult
    {
        public List<UrlHistoryContainer> Errors { get; set; }
        public List<UrlHistoryContainer> ChannelsToDelete { get; set; }
        public GifPostingResult(List<UrlHistoryContainer> errors, List<UrlHistoryContainer> channelsToDelete)
        {
            Errors = errors;
            ChannelsToDelete = channelsToDelete;
        }
    }
}
