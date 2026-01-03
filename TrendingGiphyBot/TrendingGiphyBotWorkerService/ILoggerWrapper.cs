
namespace TrendingGiphyBotWorkerService;

public interface ILoggerWrapper<T>
{
    void LogDiscordMessage(LogLevel logLevel, Discord.LogMessage logMessage);
    Task SwallowAsync(Func<Task> taskDelegate);
    Task SwallowAsync(Task task);
    void LogTopLevelException(Exception exception);
}