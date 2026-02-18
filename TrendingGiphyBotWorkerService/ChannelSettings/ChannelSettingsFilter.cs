namespace TrendingGiphyBotWorkerService.ChannelSettings;

[RegisterSingleton]
public class ChannelSettingsFilter(ILogger<ChannelSettingsFilter> _logger) : IChannelSettingsFilter
{
    public bool InPostingHours(ChannelSettingsModel channelSettings, DateTimeOffset now)
    {
        if (channelSettings.PostingHours?.From is null || channelSettings.PostingHours?.To is null)
            return true;

        var from = TimeSpan.FromHours(channelSettings.PostingHours.From.Value);
        var to = TimeSpan.FromHours(channelSettings.PostingHours.To.Value);

        var localTimeOfDay = DetermineLocalTimeOfDay();

        var result = from <= to
            ? localTimeOfDay >= from && localTimeOfDay <= to
            : localTimeOfDay >= from ^ localTimeOfDay <= to;

        _logger.LogChannelInPostingHours(channelSettings.ChannelId, result);

        return result;

        TimeSpan DetermineLocalTimeOfDay()
        {
            if (channelSettings.PostingHours.UtcOffset is null or "")
                return now.TimeOfDay;

            var offset = TimeSpan.Parse(channelSettings.PostingHours.UtcOffset);

            var nowLocal = now + offset;

            return nowLocal.TimeOfDay;
        }
    }
}
