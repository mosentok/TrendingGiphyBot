namespace TrendingGiphyBotWorkerService.Interactions;

public class UtcOffsetParser : IUtcOffsetParser
{
    public async Task<(bool Success, decimal? UtcOffset)> TryParseUtcOffsetAsync(string stringWithSign)
    {
        short sign;

        switch (stringWithSign[0])
        {
            case '+':
                sign = 1;
                break;
            case '-':
                sign = -1;
                break;
            default:
                return (false, null);
        }

        var split = stringWithSign[1..].Split([':'], 2);

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

        var decimalOffset = hours + (minutes / 100m);

        return (true, decimalOffset * sign);
    }
}
