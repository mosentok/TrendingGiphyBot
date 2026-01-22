using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.GifPostingBehavior;

public class GifPostingBehaviorHelper : IGifPostingBehaviorHelper
{
    public async Task<bool> SetBehaviorAsync(ChannelSettingsModel channelSettings, GifPostingBehaviorKind kind)
    {
        var trendingGifsOnlyId = kind.AsInt();

        if (channelSettings!.GifPostingBehaviorId == trendingGifsOnlyId)
            return false;

        channelSettings.GifPostingBehaviorId = trendingGifsOnlyId;

        return true;
    }
}
