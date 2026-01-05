namespace TrendingGiphyBotWorkerService.Giphy;

public interface IGiphyClient
{
    Task<GiphyResponse> GetTrendingGifsAsync(int? offset = 0, int? limit = 1000, string? rating = "pg", string? bundle = "clips_grid_picker", CancellationToken cancellationToken = default);
}
