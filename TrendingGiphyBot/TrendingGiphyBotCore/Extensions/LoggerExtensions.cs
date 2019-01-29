using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TrendingGiphyBotCore.Extensions
{
    public static class LoggerExtensions
    {
        public static async Task SwallowAsync(this ILogger logger, Task task)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception was thrown and swallowed.");
            }
        }
        public static async Task SwallowAsync(this ILogger logger, Func<Task> func)
        {
            try
            {
                await func.Invoke();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception was thrown and swallowed.");
            }
        }
    }
}
