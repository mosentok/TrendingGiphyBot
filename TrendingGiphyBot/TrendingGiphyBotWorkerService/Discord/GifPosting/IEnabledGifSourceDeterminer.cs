using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting;

public interface IEnabledGifSourceDeterminer
{
    List<GifSourceKind> DetermineEnabledGifSources(ChannelSettingsModel channelSetting);
}
