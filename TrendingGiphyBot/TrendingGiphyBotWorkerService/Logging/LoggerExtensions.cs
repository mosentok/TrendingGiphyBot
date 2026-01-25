using System.Runtime.CompilerServices;
using Discord;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Logging;

public static partial class LoggerExtensions
{
	[LoggerMessage(Level = LogLevel.Critical, Message = "<{Method}> Exception caught at the top level.")]
	public static partial void LogTopLevelException(this ILogger logger, Exception exception, [CallerMemberName] string method = "");

	[LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> Exception caught when attempting to save GifPost {GifPost} to the database.")]
	public static partial void LogGifPostingException(this ILogger logger, Exception exception, [LogProperties] GifPost gifPost, [CallerMemberName] string method = "");

	[LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> Exception caught when attempting to refresh the gif cache.")]
	public static partial void LogGifCacheRefreshException(this ILogger logger, Exception exception, [CallerMemberName] string method = "");

	[LoggerMessage(Message = "<{Method}> Discord.Net LogMessage: {LogMessage}")]
	public static partial void LogDiscordMessage(this ILogger logger, LogLevel logLevel, LogMessage logMessage, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Error, Message = "<{Method}> An exception caught when posting GiphyDataId {GiphyDataId} to ChannelId {ChannelId}. Removing GifPost {GifPost}.")]
    public static partial void LogErrorPostingGif(this ILogger logger, Exception exception, string giphyDataId, ulong channelId, [LogProperties] GifPost gifPost, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> {NewGifsCount} new gifs have been added to the cache.")]
    public static partial void LogGifCacheCount(this ILogger logger, int newGifsCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The gif cache is refreshing.")]
    public static partial void LogGifCacheIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The gif cache has refreshed and now contains {GifCacheCount} gifs.")]
    public static partial void LogGifCacheHasRefreshed(this ILogger logger, int gifCacheCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The gif stage is refreshing.")]
    public static partial void LogGifStageIsRefreshing(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> The gif stage has refreshed and now contains {GifStageCount} gifs.")]
    public static partial void LogGifStageHasRefreshed(this ILogger logger, int gifStageCount, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Posting gifs.")]
    public static partial void LogPostingGifs(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Posted gifs.")]
    public static partial void LogPostedGifs(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeding gif posting behaviors.")]
    public static partial void LogSeedingGifPostingBehaviors(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeded gif posting behaviors.")]
    public static partial void LogSeededGifPostingBehaviors(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeding intervals.")]
    public static partial void LogSeedingIntervals(this ILogger logger, [CallerMemberName] string method = "");

    [LoggerMessage(Level = LogLevel.Information, Message = "<{Method}> Seeded intervals.")]
    public static partial void LogSeededIntervals(this ILogger logger, [CallerMemberName] string method = "");
}
