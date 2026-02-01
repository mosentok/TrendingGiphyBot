using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

public interface IGifFinder
{
    Task<Maybe<GiphyData>> TryGetUnseenGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken);
}
