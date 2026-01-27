namespace TrendingGiphyBotWorkerService.Giphy.Api.Paging;

public interface IGiphyTrendingPager
{
    Task<List<GiphyData>> GetTrendingGifsAsync(CancellationToken cancellationToken);
}
