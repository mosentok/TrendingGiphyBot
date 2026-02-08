using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding;

public interface IGiphyDataFinder
{
    Task<Maybe<GiphyData>> TryGetUnseenGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken);
}
