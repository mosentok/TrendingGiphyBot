using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TrendingGiphyBotFunctions.Wrappers
{
    public interface IStatWrapper
    {
        Task PostStatsAsync(long botId, int guildCount, ILogger log);
    }
}