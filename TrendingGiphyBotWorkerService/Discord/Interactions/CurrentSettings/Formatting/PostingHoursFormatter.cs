using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

[RegisterSingleton]
public class PostingHoursFormatter(IUtcOffsetFormatter _utcOffsetFormatter) : IPostingHoursFormatter
{
    public string Format(PostingHours? postingHours)
    {
        if (postingHours is not { From: { } from, To: { } to, UtcOffset: { } utcOffset })
            return "Any Time";

        var utcOffsetString = _utcOffsetFormatter.FormatUtcOffset(utcOffset);

        return $"{from:D2}:00 - {to:D2}:00 UTC{utcOffsetString}";
    }
}
