using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.GifPostingBehavior;

public interface IGifPostingBehaviorHelper
{
    Task<bool> SetBehaviorAsync(ChannelSettingsModel channelSettings, GifPostingBehaviorKind kind);
}