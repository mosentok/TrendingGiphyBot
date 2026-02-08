using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;
using TrendingGiphyBotWorkerService.Results;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

public interface IKlipyTrendingGifFinder
{
    Maybe<KlipyData> TryGetTrendingGif(ChannelSettingsModel channel);
}