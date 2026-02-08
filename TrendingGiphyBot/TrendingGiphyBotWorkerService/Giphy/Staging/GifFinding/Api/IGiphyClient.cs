namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

public interface IGiphyClient
{
    Task<GiphyResponse> SearchAsync(string searchTerms, int offset = 0, int limit = 50, string rating = "pg", string lang = "en", string bundle = "clips_grid_picker", CancellationToken cancellationToken = default);
    Task<RandomResponse> GetRandomGifAsync(string rating = "pg", CancellationToken cancellationToken = default);
    Task<GiphyResponse> GetTrendingGifsAsync(int offset = 0, int limit = 1000, string rating = "pg", string bundle = "clips_grid_picker", CancellationToken cancellationToken = default);
}
