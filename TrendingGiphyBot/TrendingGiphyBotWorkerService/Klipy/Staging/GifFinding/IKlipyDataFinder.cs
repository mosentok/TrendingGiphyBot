using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

public interface IKlipyDataFinder
{
    KlipyData? TryGetUnseenGif(ChannelSettingsModel channel, CancellationToken cancellationToken);
}