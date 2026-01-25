
namespace TrendingGiphyBotWorkerService.Interactions;

public interface IUtcOffsetParser
{
    Task<(bool Success, decimal? UtcOffset)> TryParseUtcOffsetAsync(string stringWithSign);
}