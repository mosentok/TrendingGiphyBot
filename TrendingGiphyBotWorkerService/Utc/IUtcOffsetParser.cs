namespace TrendingGiphyBotWorkerService.Utc;

public interface IUtcOffsetParser
{
    (bool Success, TimeSpan? UtcOffset) TryParseUtcOffset(string utcOffsetString);
}