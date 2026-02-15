using System.Runtime.CompilerServices;

namespace TrendingGiphyBotWorkerService.Klipy.Staging;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The Klipy stage contains {KlipyStageCount} items.")]
    public static partial void LogKlipyStageCount(this ILogger logger, int klipyStageCount, [CallerMemberName] string method = "");
}
