using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public class GiphyTrendingCacheWorker(
    ILogger<GiphyTrendingCacheWorker> _logger,
    IGiphyTrendingCache _giphyTrendingCache,
    IOptionsMonitor<AppConfig> _appConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await Task.Delay(_appConfig.CurrentValue.Giphy.Staging.TrendingCaching.TimeSpanBetweenRefreshes, stoppingToken);

                _logger.LogGiphyTrendingCacheIsRefreshing();

                await _giphyTrendingCache.RefreshTrendingGifsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogTopLevelException(exception);
            }
            finally
            {
                _logger.LogGiphyTrendingCacheHasRefreshed();
            }
    }
}
