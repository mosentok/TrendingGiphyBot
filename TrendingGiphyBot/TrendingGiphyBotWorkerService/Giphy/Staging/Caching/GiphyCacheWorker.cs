using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.Caching;

public class GiphyCacheWorker(
    ILogger<GiphyCacheWorker> _logger,
    IGifCache _gifCache,
	GiphyCacheWorkerConfig _giphyCacheWorkerConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(_giphyCacheWorkerConfig.TimeSpanBetweenCacheRefreshes, stoppingToken);

                await _gifCache.RefreshAsync(stoppingToken);
            }
        }
        catch (Exception exception)
        {
            _logger.LogTopLevelException(exception);
        }
    }
}
