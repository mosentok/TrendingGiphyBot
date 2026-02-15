using System.Runtime.CompilerServices;

namespace TrendingGiphyBotWorkerService.Giphy.Staging;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The Giphy stage contains {GiphyStageCount} items.")]
    public static partial void LogGiphyStageCount(this ILogger logger, int giphyStageCount, [CallerMemberName] string method = "");
}
