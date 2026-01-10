using Discord;

namespace TrendingGiphyBotWorkerService.Logging;

public interface ILoggerWrapper<T>
{
    void LogGifPostingException(Exception exception);
    void LogDiscordMessage(LogLevel logLevel, LogMessage logMessage);
    Task SwallowAsync(Task task);
    void LogTopLevelException(Exception exception);
}