namespace TrendingGiphyBotWorkerService.Paging;

public interface IPager
{
    Task<List<T>> PageAsync<T>(SearchWithOffset<T> searchWithOffsetAsync);
}
