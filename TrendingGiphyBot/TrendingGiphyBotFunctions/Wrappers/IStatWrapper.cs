using System.Threading.Tasks;

namespace TrendingGiphyBotFunctions.Wrappers
{
    public interface IStatWrapper
    {
        Task PostStatAsync(string requestUri, string content, string token);
    }
}