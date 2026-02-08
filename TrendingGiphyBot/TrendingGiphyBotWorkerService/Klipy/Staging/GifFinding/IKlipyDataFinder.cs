using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

public interface IKlipyDataFinder
{
    Task<Maybe<KlipyData>> TryGetUnseenGifAsync(ChannelSettingsModel channel, CancellationToken cancellationToken);
}