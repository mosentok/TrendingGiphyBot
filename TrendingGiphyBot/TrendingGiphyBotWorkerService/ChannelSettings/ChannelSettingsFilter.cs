namespace TrendingGiphyBotWorkerService.ChannelSettings;

[RegisterSingleton]
public class ChannelSettingsFilter : IChannelSettingsFilter
{
    public bool InPostingHours(ChannelSettingsModel channelSettings, DateTimeOffset now)
    {
        if (!channelSettings.PostingHoursFrom.HasValue || !channelSettings.PostingHoursTo.HasValue)
            return true;

        var from = TimeSpan.FromHours(channelSettings.PostingHoursFrom.Value);
        var to = TimeSpan.FromHours(channelSettings.PostingHoursTo.Value);

        var localTimeOfDay = DetermineLocalTimeOfDay();

        return from <= to
            ? localTimeOfDay >= from && localTimeOfDay <= to
            : localTimeOfDay >= from ^ localTimeOfDay <= to;

        TimeSpan DetermineLocalTimeOfDay()
        {
            if (channelSettings.UtcOffset is null or "")
                return now.TimeOfDay;

            var offset = TimeSpan.Parse(channelSettings.UtcOffset);

            var nowLocal = now + offset;

            return nowLocal.TimeOfDay;
        }
    }
}
