using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting;

[RegisterSingleton]
public class EnabledGifSourceDeterminer : IEnabledGifSourceDeterminer
{
    public List<GifSourceKind> DetermineEnabledGifSources(ChannelSettingsModel channelSetting)
    {
        if (channelSetting.GifSource is not { } gifSource)
            return [.. Enum.GetValues<GifSourceKind>()];

        var enabled = new List<GifSourceKind>();

        if (gifSource.HasFlag(GifSourceKind.Giphy))
            enabled.Add(GifSourceKind.Giphy);

        if (gifSource.HasFlag(GifSourceKind.Klipy))
            enabled.Add(GifSourceKind.Klipy);

        return enabled;
    }
}
