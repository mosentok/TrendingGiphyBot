using System.Runtime.CompilerServices;

namespace TrendingGiphyBotWorkerService.Logging;

public static partial class LoggerExtensions
{
	[LoggerMessage(Level = LogLevel.Critical, Message = "<{Method}> Exception caught at the top level.")]
	public static partial void LogTopLevelException(this ILogger logger, Exception exception, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The gif stage is refreshing.")]
    public static partial void LogGifStageIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The gif stage has refreshed.")]
    public static partial void LogGifStageHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Initializing.")]
    public static partial void LogInitializing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Initialized.")]
    public static partial void LogInitialized(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Configuration:{DebugView}.")]
    public static partial void LogDebugView(this ILogger logger, string debugView, [CallerMemberName] string method = "");
}
