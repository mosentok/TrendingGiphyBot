using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

public interface IGiphyRandomGifFinder
{
    Task<Maybe<GiphyData>> TryGetRandomGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken);
}