namespace TrendingGiphyBotWorkerService.Paging;

public delegate Task<List<T>> SearchWithOffset<T>(int offset);
