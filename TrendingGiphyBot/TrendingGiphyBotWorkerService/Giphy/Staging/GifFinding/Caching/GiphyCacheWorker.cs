using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching;

public class GiphyCacheWorker(
    ILogger<GiphyCacheWorker> _logger,
    IGiphyTrendingCache _giphyTrendingCache,
    IOptions<AppConfig> _appConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await Task.Delay(_appConfig.Value.Giphy.Staging.Caching.TimeSpanBetweenRefreshes, stoppingToken);

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
