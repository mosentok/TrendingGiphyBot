using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

public class KlipyCacheWorker
(
    ILogger<KlipyCacheWorker> _logger,
    KlipyTrendingCache _klipyTrendingCache,
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

                await _klipyTrendingCache.RefreshTrendingGifsAsync(stoppingToken);
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
