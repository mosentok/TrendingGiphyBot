using TrendingGiphyBotWorkerService.Giphy;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IGifPostStage
{
    GiphyData GetStagedGiphyData(ulong channelId);
    bool HasStagedGiphyData(ulong channelId);
    bool TryGetGiphyData(ulong channelId, out GiphyData? giphyData);
    void Refresh();
    void Evict(ulong channelId);
}
