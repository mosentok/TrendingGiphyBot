namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Api;

public class GiphyResponse
{
    public required List<GiphyData> Data { get; set; }
    public required Pagination Pagination { get; set; }
}
