using System.Collections.Immutable;
using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public interface IGifPostStage
{
    IImmutableDictionary<ulong, GiphyData> GetChannelGifPostStage();
    Task RefreshAsync();
    void Evict(ulong channelId);
}
