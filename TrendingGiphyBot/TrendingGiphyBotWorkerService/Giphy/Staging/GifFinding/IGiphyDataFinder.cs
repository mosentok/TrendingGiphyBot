using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

public interface IGiphyDataFinder
{
    Task<GiphyDataWithSource?> TryGetUnseenGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken);
}
