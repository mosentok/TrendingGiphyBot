using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

public interface IKlipyDataFinder
{
    KlipyDataWithSource? TryGetUnseenGif(ChannelSettingsModel channel, CancellationToken cancellationToken);
}