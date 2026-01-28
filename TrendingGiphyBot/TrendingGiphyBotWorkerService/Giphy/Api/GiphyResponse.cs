namespace TrendingGiphyBotWorkerService.Giphy.Api;

public class GiphyResponse
{
    public required List<GiphyData> Data { get; set; }
    public required Pagination Pagination { get; set; }
}
