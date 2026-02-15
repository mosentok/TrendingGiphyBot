using System.Runtime.CompilerServices;

namespace TrendingGiphyBotWorkerService.Intervals;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeding intervals.")]
    public static partial void LogSeedingIntervals(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeded intervals.")]
    public static partial void LogSeededIntervals(this ILogger logger, [CallerMemberName] string method = "");
}
