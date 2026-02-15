using System.Runtime.CompilerServices;

namespace TrendingGiphyBotWorkerService.GifPostingBehavior;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeding gif posting behaviors.")]
    public static partial void LogSeedingGifPostingBehaviors(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeded gif posting behaviors.")]
    public static partial void LogSeededGifPostingBehaviors(this ILogger logger, [CallerMemberName] string method = "");
}
