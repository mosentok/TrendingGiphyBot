using Microsoft.Extensions.Logging;
using System;

namespace TrendingGiphyBotFunctions.Wrappers
{
    public class LoggerWrapper : ILoggerWrapper
    {
        readonly ILogger _Log;
        public LoggerWrapper(ILogger log)
        {
            _Log = log;
        }
        public void LogInformation(string message)
        {
            _Log.LogInformation(message);
        }
        public void LogError(string message)
        {
            _Log.LogError(message);
        }
        public void LogError(Exception exception, string message)
        {
            _Log.LogError(exception, message);
        }
    }
}
