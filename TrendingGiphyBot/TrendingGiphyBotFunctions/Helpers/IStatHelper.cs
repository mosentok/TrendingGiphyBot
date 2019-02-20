using System.Threading.Tasks;

namespace TrendingGiphyBotFunctions.Helpers
{
    public interface IStatHelper
    {
        Task PostStatAsync(string requestUri, string content, string token);
    }
}