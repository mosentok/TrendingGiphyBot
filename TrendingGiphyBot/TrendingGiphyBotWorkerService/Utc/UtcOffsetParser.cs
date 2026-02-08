namespace TrendingGiphyBotWorkerService.Utc;

[RegisterSingleton]
public class UtcOffsetParser : IUtcOffsetParser
{
    public async Task<(bool Success, TimeSpan? UtcOffset)> TryParseUtcOffsetAsync(string utcOffsetString)
    {
        short sign;

        switch (utcOffsetString[0])
        {
            case '+':
                sign = 1;

                break;

            case '-':
                sign = -1;

                break;

            default:
                if (utcOffsetString.Length != 5)
                    return (false, null);

                sign = 1;

                break;
        }

        var utcOffsetWithoutSignString = utcOffsetString.Length == 5
            ? utcOffsetString
            : utcOffsetString[1..];

        var split = utcOffsetWithoutSignString.Split([':'], 2);

        if (split.Length != 2)
            return (false, null);

        var hoursSuccess = !int.TryParse(split[0], out var hours);

        if (hoursSuccess)
            return (false, null);

        var minutesSuccess = !int.TryParse(split[1], out var minutes);

        if (minutesSuccess)
            return (false, null);

        if (hours < 0 || hours > 14 || minutes < 0 || minutes > 59)
            return (false, null);

        var utcOffset = new TimeSpan(hours, minutes, 0) * sign;

        return (true, utcOffset);
    }

    public string FormatUtcOffsetString(string utcOffsetString)
    {
        var utcOffset = TimeSpan.Parse(utcOffsetString);

        return utcOffset >= TimeSpan.Zero
            ? $"+{utcOffset:hh\\:mm}"
            : $"-{utcOffset:hh\\:mm}";
    }
}
