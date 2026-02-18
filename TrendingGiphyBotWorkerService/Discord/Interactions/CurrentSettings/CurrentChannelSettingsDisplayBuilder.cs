using System.Text;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings;

[RegisterSingleton]
public class CurrentChannelSettingsDisplayBuilder(
    IOptionsMonitor<AppConfig> _appConfig,
    IHowOftenFormatter _howOftenFormatter,
    IGifSourcesFormatter _gifSourcesFormatter,
    IUtcOffsetFormatter _utcOffsetFormatter
) : ICurrentChannelSettingsDisplayBuilder
{
    public string BuildUnifiedSettingsDisplay(ChannelSettingsDto channelSettings)
    {
        if (channelSettings.Interval == Interval.None && channelSettings.GifSource == GifSourceKind.None)
            return "Never post gifs. The bot is set to **never post gifs**, and no **gif sources** are selected, either.";

        if (channelSettings.Interval == Interval.None)
            return "Never post gifs. The bot is set to **never post gifs**.";

        if (channelSettings.GifSource == GifSourceKind.None)
            return "Never post gifs. No **gif sources** are selected.";

        var howOftenValue = _howOftenFormatter.Format(channelSettings.Frequency, channelSettings.Interval);
        var gifSourcesValue = _gifSourcesFormatter.Format(channelSettings.GifSource);
        var effectiveRetentionDays = channelSettings.RetentionDays ?? _appConfig.CurrentValue.GifRetention.DefaultDays;

        switch (channelSettings.GifPostingBehavior)
        {
            case GifPostingBehaviorKind.TrendingGifsOnly:
                var trendingOnlyPostingHoursMessage = BuildPostingHoursClause();

                return $"Post trending gifs from **{gifSourcesValue}** every **{howOftenValue}**. Remember them for up to **{effectiveRetentionDays}** days. {trendingOnlyPostingHoursMessage}";

            case GifPostingBehaviorKind.TrendingGifsWithRandomGifs:
                var hasKeyword = !string.IsNullOrWhiteSpace(channelSettings.GifKeyword);
                var trendingWithRandomPostingHoursMessage = BuildPostingHoursClause();

                var randomGifsClause = hasKeyword
                    ? $"post **random gifs** of **{channelSettings.GifKeyword}**"
                    : "post **random gifs**";

                return $"Post trending gifs from **{gifSourcesValue}** every **{howOftenValue}**. When there's no new trending gifs, {randomGifsClause}. Remember them for up to **{effectiveRetentionDays}** days. {trendingWithRandomPostingHoursMessage}";

            default:
                return "Never post gifs.";
        }

        string BuildPostingHoursClause()
        {
            if (channelSettings.PostingHours is not { From: { } from, To: { } to, UtcOffset: { } utcOffset })
                return "Gifs can be posted anytime.";

            var utcOffsetString = _utcOffsetFormatter.FormatUtcOffset(utcOffset);

            return $"Only post gifs from **{from:D2}:00** to **{to:D2}:00** **UTC{utcOffsetString}**.";
        }
    }
}
