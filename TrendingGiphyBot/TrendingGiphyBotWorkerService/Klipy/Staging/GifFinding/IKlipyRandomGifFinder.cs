using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

public interface IKlipyRandomGifFinder
{
    KlipyData? TryGetRandomGif(ChannelSettingsModel channel, CancellationToken cancellationToken);
}