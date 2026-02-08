using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

public interface IKlipyRandomGifFinder
{
    Task<Maybe<KlipyData>> TryGetRandomGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken);
}