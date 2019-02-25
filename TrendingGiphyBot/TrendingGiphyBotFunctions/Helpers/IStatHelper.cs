using System.Threading.Tasks;

namespace TrendingGiphyBotFunctions.Helpers
{
    //TODO rename this to Wrapper
    public interface IStatHelper
    {
        Task PostStatAsync(string requestUri, string content, string token);
    }
}