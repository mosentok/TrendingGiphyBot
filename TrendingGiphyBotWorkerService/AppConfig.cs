using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Discord;
using TrendingGiphyBotWorkerService.Discord.GifPosting.Delaying;
using TrendingGiphyBotWorkerService.Giphy;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Klipy;
using TrendingGiphyBotWorkerService.Logging;
using TrendingGiphyBotWorkerService.Paging;

namespace TrendingGiphyBotWorkerService;

public class AppConfig
{
    public required AttributionConfig Attribution { get; set; }
    public required DelayerConfig Delayer { get; set; }
    public required DiscordConfig Discord { get; set; }
    public required GiphyConfig Giphy { get; set; }
    public required IntervalConfig Intervals { get; set; }
    public required KlipyConfig Klipy { get; set; }
    public required LoggingConfig Logging { get; set; }
    public required PagerConfig Pager { get; set; }
    public required RetentionDaysConfig GifRetention { get; set; }
    public required StartupConfig Startup { get; set; }
}
