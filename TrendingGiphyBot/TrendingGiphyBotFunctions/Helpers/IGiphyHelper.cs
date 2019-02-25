using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Models;

namespace TrendingGiphyBotFunctions.Helpers
{
    //TODO rename this to Wrapper
    public interface IGiphyHelper
    {
        Task<GiphyRandomResponse> GetRandomGifAsync(string giphyRandomEndpoint, string tag);
        Task<GiphyTrendingResponse> GetTrendingGifsAsync(string giphyTrendingEndpoint);
    }
}