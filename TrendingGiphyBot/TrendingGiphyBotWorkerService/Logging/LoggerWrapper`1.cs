using Discord;

namespace TrendingGiphyBotWorkerService.Logging;

// TODO replace this and unit test with Microsoft.Extensions.Logging.Testing.FakeLogger?
public class LoggerWrapper<T>(ILogger<T> _logger) : ILoggerWrapper<T>
{
	public void LogTopLevelException(Exception exception) => _logger.LogTopLevelException(exception);

	public void LogGifPostingException(Exception exception) => _logger.LogGifPostingException(exception);

	public void LogDiscordMessage(LogLevel logLevel, LogMessage logMessage) => _logger.LogDiscordMessage(logLevel, logMessage);

	public void LogThatChannelIsNotStaged(ulong channelId) => _logger.LogThatChannelIsNotStaged(channelId);
}
