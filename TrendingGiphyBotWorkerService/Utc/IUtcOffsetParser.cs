namespace TrendingGiphyBotWorkerService.Utc;

public interface IUtcOffsetParser
{
    string FormatUtcOffsetString(string utcOffsetString);
    Task<(bool Success, TimeSpan? UtcOffset)> TryParseUtcOffsetAsync(string utcOffsetString);
}