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

        return from <= to
            ? hour >= from && hour <= to
            : hour >= from ^ hour <= to;

        int DetermineLocalHour()
        {
            if (channelSettings.UtcOffset is null)
                return now.Hour;

            var offsetDecimal = channelSettings.UtcOffset.Value;
            var sign = Math.Sign(offsetDecimal);
            var abs = Math.Abs(offsetDecimal);
            var offsetHours = (int)Math.Truncate(abs);
            var offsetMinutes = (int)Math.Round((abs - offsetHours) * 100);

            var offset = TimeSpan.FromHours(offsetHours) + TimeSpan.FromMinutes(offsetMinutes);

            if (sign < 0)
                offset = -offset;

            var local = now + offset;

            return local.Hour;
        }
    }
}
