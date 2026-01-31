using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public class GiphyCacheWorker(
    ILogger<GiphyCacheWorker> _logger,
    IGiphyTrendingCache _giphyTrendingCache,
    GiphyCacheWorkerConfig _giphyCacheWorkerConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await Task.Delay(_giphyCacheWorkerConfig.TimeSpanBetweenCacheRefreshes, stoppingToken);

                _logger.LogGifTrendingCacheIsRefreshing();

                await _giphyTrendingCache.RefreshTrendingGifsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogTopLevelException(exception);
            }
            finally
            {
                _logger.LogGifTrendingCacheHasRefreshed();
            }
    }
}
