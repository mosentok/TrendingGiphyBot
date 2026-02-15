namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

public interface IGiphyClient
{
    Task<GiphyResponse> SearchAsync(
        string searchTerms,
        string rating,
        int offset = 0,
        int limit = 50,
        string lang = "en",
        string bundle = "clips_grid_picker",
        CancellationToken cancellationToken = default);
    Task<RandomResponse> GetRandomGifAsync(string rating, CancellationToken cancellationToken = default);
    Task<GiphyResponse> GetTrendingGifsAsync(
        string rating,
        int offset = 0,
        int limit = 1000,
        string bundle = "clips_grid_picker",
        CancellationToken cancellationToken = default);
}
