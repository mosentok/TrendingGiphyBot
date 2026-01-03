using Discord;

namespace TrendingGiphyBotWorkerService;

public static partial class LoggerExtensions
{
	[LoggerMessage(Level = LogLevel.Critical, Message = "Exception caught at the top level.")]
	public static partial void LogTopLevelException(this ILogger logger, Exception exception);

	[LoggerMessage(Message = "{LogMessage}")]
	public static partial void LogDiscordMessage(this ILogger logger, LogLevel logLevel, LogMessage logMessage);
}
