using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Giphy.Api;

namespace TrendingGiphyBotWorkerService.Giphy.Staging
{
    public interface IGiphyRandomGifFinder
    {
        Task<Maybe<GiphyData>> TryGetRandomGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken);
    }
}