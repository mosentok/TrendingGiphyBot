using TrendingGiphyBotWorkerService.Giphy;

namespace TrendingGiphyBotWorkerService.Discord;

public interface IGifPostStage
{
    GiphyData GetStagedGiphyData(ulong channelId);
    bool HasStagedGiphyData(ulong channelId);
    Task RefreshAsync();
    void Evict(ulong channelId);
}
