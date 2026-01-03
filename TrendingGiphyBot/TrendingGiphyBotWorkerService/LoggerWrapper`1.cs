using Discord;

namespace TrendingGiphyBotWorkerService;

public class LoggerWrapper<T>(ILogger<T> _logger) : ILoggerWrapper<T>
{
	public void LogTopLevelException(Exception exception) => _logger.LogTopLevelException(exception);

	public void LogDiscordMessage(LogLevel logLevel, LogMessage logMessage) => _logger.LogDiscordMessage(logLevel, logMessage);

	//TODO confirm if the discord socket client's native logging serves the purpose of these swallow methods
	public async Task SwallowAsync(Task task)
	{
		try
		{
			await task;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An exception was thrown and swallowed.");
		}
	}
}
