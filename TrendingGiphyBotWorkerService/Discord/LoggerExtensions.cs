using System.Runtime.CompilerServices;
using Discord;

namespace TrendingGiphyBotWorkerService.Discord;

public static partial class LoggerExtensions
{
	[LoggerMessage(Message = "<{Method}> Discord.Net LogMessage: {LogMessage}")]
	public static partial void LogDiscordMessage(this ILogger logger, LogLevel logLevel, LogMessage logMessage, [CallerMemberName] string method = "");
}
