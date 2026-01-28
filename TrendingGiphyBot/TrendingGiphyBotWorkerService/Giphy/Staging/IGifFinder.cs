using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public interface IGifFinder
{
    Task<Maybe<GiphyData>> TryGetUnseenGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken);
}
