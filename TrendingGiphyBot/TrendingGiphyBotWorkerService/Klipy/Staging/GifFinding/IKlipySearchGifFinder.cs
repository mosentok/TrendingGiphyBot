using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Api;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding;

public interface IKlipySearchGifFinder
{
    KlipyData? TryGetSearchGif(ChannelSettingsModel channel);
}