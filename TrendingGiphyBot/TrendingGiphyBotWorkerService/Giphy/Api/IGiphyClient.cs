namespace TrendingGiphyBotWorkerService.Giphy.Api;

public interface IGiphyClient
{
    Task<RandomResponse> GetRandomGifAsync(string? keyword = null, string? rating = "pg", CancellationToken cancellationToken = default);
    Task<TrendingResponse> GetTrendingGifsAsync(int? offset = 0, int? limit = 1000, string? rating = "pg", string? bundle = "clips_grid_picker", CancellationToken cancellationToken = default);
}
