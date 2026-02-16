using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

public interface IGifSourcesFormatter
{
    string Format(GifSourceKind? gifSource);
}
