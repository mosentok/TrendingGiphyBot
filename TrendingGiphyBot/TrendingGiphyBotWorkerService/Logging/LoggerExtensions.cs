using Discord;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Logging;

public static partial class LoggerExtensions
{
	[LoggerMessage(Level = LogLevel.Critical, Message = "Exception caught at the top level.")]
	public static partial void LogTopLevelException(this ILogger logger, Exception exception);

	[LoggerMessage(Level = LogLevel.Error, Message = "Exception caught when attempting to save GifPost {GifPost} to the database.")]
	public static partial void LogGifPostingException(this ILogger logger, Exception exception, [LogProperties] GifPost gifPost);

	[LoggerMessage(Level = LogLevel.Error, Message = "Exception caught when attempting to refresh the gif cache.")]
	public static partial void LogGifCacheRefreshException(this ILogger logger, Exception exception);

	[LoggerMessage(Message = "Discord.Net LogMessage: {LogMessage}")]
	public static partial void LogDiscordMessage(this ILogger logger, LogLevel logLevel, LogMessage logMessage);

    [LoggerMessage(Level = LogLevel.Error, Message = "An exception caught when posting GiphyDataId {GiphyDataId} to ChannelId {ChannelId}. Removing GifPost {GifPost}.")]
    public static partial void LogErrorPostingGif(this ILogger logger, Exception exception, string giphyDataId, ulong channelId, [LogProperties] GifPost gifPost);

    [LoggerMessage(Level = LogLevel.Information, Message = "{NewGifsCount} new gifs have been added to the cache.")]
    public static partial void LogGifCacheCount(this ILogger logger, int newGifsCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "The gif cache is refreshing.")]
    public static partial void LogGifCacheIsRefreshing(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "The gif cache has refreshed and now contains {GifCacheCount} gifs.")]
    public static partial void LogGifCacheHasRefreshed(this ILogger logger, int gifCacheCount);

    [LoggerMessage(Level = LogLevel.Information, Message = "The gif stage is refreshing.")]
    public static partial void LogGifStageIsRefreshing(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "The gif stage has refreshed and now contains {GifStageCount} gifs.")]
    public static partial void LogGifStageHasRefreshed(this ILogger logger, int gifStageCount);
}
