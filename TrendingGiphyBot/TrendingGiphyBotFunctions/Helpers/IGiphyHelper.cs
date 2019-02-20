using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Models;

namespace TrendingGiphyBotFunctions.Helpers
{
    public interface IGiphyHelper
    {
        Task<GiphyRandomResponse> GetRandomGifAsync(string giphyRandomEndpoint, string tag);
        Task<GiphyTrendingResponse> GetTrendingGifsAsync(string giphyTrendingEndpoint);
    }
}