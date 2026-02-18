using System.Text;
using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings;

[RegisterSingleton]
public class CurrentChannelSettingsDisplayBuilder(
    IOptionsMonitor<AppConfig> _appConfig,
    IHowOftenFormatter _howOftenFormatter,
    IGifSourcesFormatter _gifSourcesFormatter,
    IPostingHoursFormatter _postingHoursFormatter
) : ICurrentChannelSettingsDisplayBuilder
{
    public string BuildMainSettingsDisplay(ChannelSettingsDto channelSettings)
    {
        var howOftenValue = _howOftenFormatter.Format(channelSettings.Frequency, channelSettings.Interval);

        return new StringBuilder()
            .AppendLine($"-# **How Often** {howOftenValue}")
            .AppendLine($"-# **Posting Behavior** {channelSettings.GifPostingBehaviorDescription}")
            .ToString();
    }

    public string BuildOptionalSettingsDisplay(ChannelSettingsDto channelSettings)
    {
        var gifSourcesValue = _gifSourcesFormatter.Format(channelSettings.GifSource);
        var effectiveRetentionDays = channelSettings.RetentionDays ?? _appConfig.CurrentValue.GifRetention.DefaultDays;
        var effectiveGiphyRating = channelSettings.GiphyRating ?? "pg";
        var effectiveGifKeyword = string.IsNullOrWhiteSpace(channelSettings.GifKeyword) ? "None" : channelSettings.GifKeyword;
        var postingHoursValue = _postingHoursFormatter.Format(channelSettings.PostingHours);

        var stringBuilder = new StringBuilder()
            .AppendLine($"-# **Gif Sources** {gifSourcesValue}")
            .AppendLine($"-# **Retention Days** {effectiveRetentionDays}");

        if (_appConfig.CurrentValue.Giphy.Staging.EnableRating)
            stringBuilder.AppendLine($"-# **Giphy Rating** {effectiveGiphyRating.ToUpper()}");

        stringBuilder
            .AppendLine($"-# **Gif Keyword** {effectiveGifKeyword}")
            .Append($"-# **Posting Hours** {postingHoursValue}");

        return stringBuilder.ToString();
    }
}
