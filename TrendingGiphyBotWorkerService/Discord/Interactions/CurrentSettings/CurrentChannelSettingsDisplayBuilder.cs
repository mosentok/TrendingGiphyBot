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
    public string BuildDisplay(ChannelSettingsDto channelSettings)
    {
        var howOftenValue = _howOftenFormatter.Format(channelSettings.Frequency, channelSettings.IntervalId);
        var gifSourcesValue = _gifSourcesFormatter.Format(channelSettings.GifSource);
        var effectiveRetentionDays = channelSettings.RetentionDays ?? _appConfig.CurrentValue.GifRetention.DefaultDays;
        var effectiveGiphyRating = channelSettings.GiphyRating ?? "pg";
        var effectiveGifKeyword = string.IsNullOrWhiteSpace(channelSettings.GifKeyword) ? "None" : channelSettings.GifKeyword;
        var postingHoursValue = _postingHoursFormatter.Format(channelSettings.PostingHoursFrom, channelSettings.PostingHoursTo);

        return string.Join(
            Environment.NewLine,
            $"-# **How Often** {howOftenValue}",
            $"-# **Posting Behavior** {channelSettings.GifPostingBehaviorDescription}",
            $"-# **Gif Sources** {gifSourcesValue}",
            $"-# **Retention Days** {effectiveRetentionDays}",
            $"-# **Giphy Rating** {effectiveGiphyRating.ToUpper()}",
            $"-# **Gif Keyword** {effectiveGifKeyword}",
            $"-# **Posting Hours** {postingHoursValue}"
        );
    }
}
