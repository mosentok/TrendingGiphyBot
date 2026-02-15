using System.Runtime.CompilerServices;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The search Giphy cache contains {SearchGiphyCacheCount} items.")]
    public static partial void LogGiphySearchCacheCount(this ILogger logger, int searchGiphyCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random Giphy cache is refreshing.")]
    public static partial void LogGiphyRandomCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random Giphy cache has refreshed.")]
    public static partial void LogGiphyRandomCacheHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The search Giphy cache is refreshing.")]
    public static partial void LogGiphySearchCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The search Giphy cache has refreshed.")]
    public static partial void LogGiphySearchCacheHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending Giphy cache is refreshing.")]
    public static partial void LogGiphyTrendingCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending Giphy cache has refreshed.")]
    public static partial void LogGiphyTrendingCacheHasRefreshed(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The trending Giphy cache contains {TrendingGiphyCacheCount} items.")]
    public static partial void LogGiphyTrendingCacheCount(this ILogger logger, int trendingGiphyCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The random Giphy cache contains {RandomGiphyCacheCount} items.")]
    public static partial void LogGiphyRandomCacheCount(this ILogger logger, int randomGiphyCacheCount, [CallerMemberName] string method = "");
}
