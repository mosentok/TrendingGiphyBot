using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting;

[RegisterSingleton]
public class EnabledGifSourceDeterminer : IEnabledGifSourceDeterminer
{
    public List<GifSourceKind> DetermineEnabledGifSources(ChannelSettingsModel channelSetting)
    {
        var enabled = new List<GifSourceKind>();

        if (channelSetting.GifSource.HasFlag(GifSourceKind.Giphy))
            enabled.Add(GifSourceKind.Giphy);

        if (channelSetting.GifSource.HasFlag(GifSourceKind.Klipy))
            enabled.Add(GifSourceKind.Klipy);

        return enabled;
    }
}
