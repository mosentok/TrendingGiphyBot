using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;
using TrendingGiphyBotWorkerService.Utc;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

[RegisterSingleton]
public class ChannelSettingsDisplay(
    IOptionsMonitor<AppConfig> _appConfig,
    IUtcOffsetParser _utcOffsetParser
) : IChannelSettingsDisplay
{
    public string DetermineGifRetentionDisplay(ChannelSettingsModel channelSettings)
    {
        var retentionDays = channelSettings.RetentionDays ?? _appConfig.CurrentValue.GifRetention.DefaultDays;

        return retentionDays == 1 ? "1 day" : $"{retentionDays} days";
    }

    public string DetermineGifSourceDisplay(ChannelSettingsModel channelSettings)
    {
        if (channelSettings.GifSource is null)
            return "<none>";

        var sources = new List<string>();

        if ((channelSettings.GifSource & GifSourceKind.Giphy) != 0)
            sources.Add("Giphy");

        if ((channelSettings.GifSource & GifSourceKind.Klipy) != 0)
            sources.Add("Klipy");

        return sources.Count == 0 ? "<none>" : string.Join(" & ", sources);
    }

    public string DetermineGiphyRatingDisplay(ChannelSettingsModel channelSettings)
    {
        var rating = channelSettings.GiphyRating ?? "pg";

        return rating.ToLower() switch
        {
            "g" => "G",
            "pg" => "PG",
            "pg-13" => "PG-13",
            "r" => "R",
            "all" => "All",
            _ => "<unknown>"
        };
    }

    public string DetermineHowOftenDisplay(ChannelSettingsModel channelSettings)
    {
        if (channelSettings.IntervalId == (int)IntervalDescription.None || channelSettings.Frequency == 0)
            return string.Empty;

        var intervalDescription = (IntervalDescription)channelSettings.IntervalId;

        return intervalDescription switch
        {
            IntervalDescription.Minutes =>
                channelSettings.Frequency == 1 ? "Every 1 Minute" : $"Every {channelSettings.Frequency} Minutes",
            IntervalDescription.Hours =>
                channelSettings.Frequency == 1 ? "Every 1 Hour" : $"Every {channelSettings.Frequency} Hours",
            _ => "<unknown>"
        };
    }

    public string DeterminePostingBehaviorDisplay(ChannelSettingsModel channelSettings)
    {
        var behaviorKind = (GifPostingBehaviorKind)channelSettings.GifPostingBehaviorId;

        return behaviorKind switch
        {
            GifPostingBehaviorKind.TrendingGifsOnly => "Trending Only",
            GifPostingBehaviorKind.TrendingGifsWithRandomGifs => "Trending With Random",
            _ => "<unknown>"
        };
    }

    public string DeterminePostingHoursDisplay(ChannelSettingsModel channelSettings)
    {
        if (channelSettings.PostingHoursFrom is null || channelSettings.PostingHoursTo is null)
            return string.Empty;

        if (channelSettings.UtcOffset is null or "")
            return $"{channelSettings.PostingHoursFrom}-{channelSettings.PostingHoursTo}";

        var formattedUtcOffset = _utcOffsetParser.FormatUtcOffsetString(channelSettings.UtcOffset);

        return $"{channelSettings.PostingHoursFrom}-{channelSettings.PostingHoursTo} ({formattedUtcOffset})";
    }

    public string TrimLabelTo80Chars(string label)
    {
        const int maxLength = 80;

        if (label.Length <= maxLength)
            return label;

        return label[..(maxLength - 3)] + "...";
    }
}
