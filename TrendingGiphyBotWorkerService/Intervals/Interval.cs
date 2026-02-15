using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Intervals;

public class Interval
{
    public int IntervalId { get; set; }
    public required string Description { get; set; }
    public virtual ICollection<ChannelSettingsModel>? ChannelSettings { get; set; }
}
