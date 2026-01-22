using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.GifPostingBehavior;

public class GifPostingBehaviorModel
{
    public int GifPostingBehaviorId { get; set; }
    public required string Description { get; set; }
    public virtual ICollection<ChannelSettingsModel> ChannelSettings { get; set; } = null!;
}
