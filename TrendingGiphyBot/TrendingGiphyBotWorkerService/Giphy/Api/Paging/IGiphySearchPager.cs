namespace TrendingGiphyBotWorkerService.Giphy.Api.Paging;

public interface IGiphySearchPager
{
    Task<List<GiphyData>> SearchAsync(string searchTerms, CancellationToken cancellationToken);
}
