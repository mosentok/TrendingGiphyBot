using Discord;

namespace TrendingGiphyBotWorkerService.Logging;

public interface ILoggerWrapper<T>
{
    void LogThatChannelIsNotStaged(ulong channelId);
    void LogGifPostingException(Exception exception);
    void LogDiscordMessage(LogLevel logLevel, LogMessage logMessage);
    void LogTopLevelException(Exception exception);
}