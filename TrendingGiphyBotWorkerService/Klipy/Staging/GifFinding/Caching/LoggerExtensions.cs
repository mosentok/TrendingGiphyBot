using System.Runtime.CompilerServices;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The search Klipy cache contains {SearchKlipyCacheCount} Klipy items.")]
    public static partial void LogKlipySearchCacheCount(this ILogger logger, int searchKlipyCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random Klipy cache is refreshing.")]
    public static partial void LogKlipyRandomCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random Klipy cache has refreshed.")]
    public static partial void LogKlipyRandomCacheHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The search Klipy cache is refreshing.")]
    public static partial void LogKlipySearchCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The search Klipy cache has refreshed.")]
    public static partial void LogKlipySearchCacheHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending Klipy cache is refreshing.")]
    public static partial void LogKlipyTrendingCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending Klipy cache has refreshed.")]
    public static partial void LogKlipyTrendingCacheHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending Klipy cache contains {TrendingKlipyCacheCount} items.")]
    public static partial void LogKlipyTrendingCacheCount(this ILogger logger, int trendingKlipyCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random Klipy cache contains {RandomKlipyCacheCount} items.")]
    public static partial void LogKlipyRandomCacheCount(this ILogger logger, int randomKlipyCacheCount, [CallerMemberName] string method = "");
}
