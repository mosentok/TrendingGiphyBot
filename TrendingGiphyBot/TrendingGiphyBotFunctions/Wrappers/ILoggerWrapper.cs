using System;

namespace TrendingGiphyBotFunctions.Wrappers
{
    public interface ILoggerWrapper
    {
        void LogInformation(string message);
        void LogError(string message);
        void LogError(Exception exception, string message);
    }
}
