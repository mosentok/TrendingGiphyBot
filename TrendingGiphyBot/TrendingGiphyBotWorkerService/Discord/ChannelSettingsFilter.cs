using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord;

public class ChannelSettingsFilter : IChannelSettingsFilter
{
    public bool InPostingHours(ChannelSettingsModel channelSettings, DateTimeOffset now)
    {
        if (!channelSettings.PostingHoursFrom.HasValue || !channelSettings.PostingHoursTo.HasValue)
            return true;

        var from = channelSettings.PostingHoursFrom.Value;
        var to = channelSettings.PostingHoursTo.Value;

        var hour = DetermineLocalHour();

        // TODO i don't think this is 100% correct. just because the hour passes the check doesn't mean the minutes do
        return from <= to
            ? hour >= from && hour <= to
            : hour >= from ^ hour <= to;

        int DetermineLocalHour()
        {
            if (channelSettings.UtcOffset is null or "")
                return now.Hour;

            var offset = TimeSpan.Parse(channelSettings.UtcOffset);

            var local = now + offset;

            return local.Hour;
        }
    }
}
