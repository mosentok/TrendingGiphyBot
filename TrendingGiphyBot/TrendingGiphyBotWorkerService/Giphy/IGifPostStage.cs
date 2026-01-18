using System.Collections.Immutable;

namespace TrendingGiphyBotWorkerService.Giphy;

public interface IGifPostStage
{
    IImmutableDictionary<ulong, GiphyData> GetChannelGifPostStage();
    Task RefreshAsync();
    void Evict(ulong channelId);
}
