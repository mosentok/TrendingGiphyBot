using TrendingGiphyBotWorkerService.Delaying;
using TrendingGiphyBotWorkerService.Discord;
using TrendingGiphyBotWorkerService.Giphy;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Paging;

namespace TrendingGiphyBotWorkerService;

public class AppConfig
{
    public required DelayerConfig Delayer { get; set; }
    public required DiscordConfig Discord { get; set; }
    public required GiphyConfig Giphy { get; set; }
    public required IntervalConfig Intervals { get; set; }
    public required PagerConfig Pager { get; set; }
}
