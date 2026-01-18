using Discord;

namespace TrendingGiphyBotWorkerService;

public static partial class LoggerExtensions
{
	[LoggerMessage(Level = LogLevel.Critical, Message = "Exception caught at the top level.")]
	public static partial void LogTopLevelException(this ILogger logger, Exception exception);

	[LoggerMessage(Level = LogLevel.Error, Message = "Exception caught when attempting to post a gif.")]
	public static partial void LogGifPostingException(this ILogger logger, Exception exception);

	[LoggerMessage(Level = LogLevel.Error, Message = "Exception caught when attempting to refresh the gif cache.")]
	public static partial void LogGifCacheRefreshException(this ILogger logger, Exception exception);

	[LoggerMessage(Message = "Discord.Net LogMessage: {LogMessage}")]
	public static partial void LogDiscordMessage(this ILogger logger, LogLevel logLevel, LogMessage logMessage);

	[LoggerMessage(Level = LogLevel.Debug, Message = "Channel with ChannelId {ChannelId} does not have staged Giphy data.")]
	public static partial void LogThatChannelIsNotStaged(this ILogger logger, ulong channelId);
}
