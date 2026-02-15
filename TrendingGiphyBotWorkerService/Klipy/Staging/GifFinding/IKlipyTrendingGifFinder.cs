using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

public interface IKlipyTrendingGifFinder
{
    KlipyData? TryGetTrendingGif(ChannelSettingsModel channel);
}