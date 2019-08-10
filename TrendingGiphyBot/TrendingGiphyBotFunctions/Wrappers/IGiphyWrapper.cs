using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Models;

namespace TrendingGiphyBotFunctions.Wrappers
{
    public interface IGiphyWrapper
    {
        Task<GiphyRandomResponse> GetRandomGifAsync(string tag);
        Task<GiphyTrendingResponse> GetTrendingGifsAsync();
    }
}