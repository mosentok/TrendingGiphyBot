using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

[RegisterSingleton]
public class GifSourcesFormatter : IGifSourcesFormatter
{
    public string Format(GifSourceKind? gifSource)
    {
        if (gifSource is null)
            return "Giphy & Klipy";

        if (gifSource.Value == GifSourceKind.None)
            return "No sources";

        var sources = new List<string>();

        if (gifSource.Value.HasFlag(GifSourceKind.Giphy))
            sources.Add("Giphy");

        if (gifSource.Value.HasFlag(GifSourceKind.Klipy))
            sources.Add("Klipy");

        return string.Join(" & ", sources);
    }
}
