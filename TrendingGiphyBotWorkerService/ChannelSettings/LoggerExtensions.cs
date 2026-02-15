using System.Runtime.CompilerServices;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "<{Method}> Valid minutes {ValidMinutes}.")]
    public static partial void LogValidMinutes(this ILogger logger, int[] validMinutes, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Trace, Message = "<{Method}> Valid hours {ValidHours}.")]
    public static partial void LogValidHours(this ILogger logger, int[] validHours, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Debug, Message = "<{Method}> Channel {ChannelId}: InPostingHours={InPostingHours}.")]
    public static partial void LogChannelInPostingHours(this ILogger logger, ulong channelId, bool inPostingHours, [CallerMemberName] string method = "");
}
