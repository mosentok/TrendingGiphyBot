using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Configuration;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Trending;

public class KlipyTrendingCacheWorker
(
    ILogger<KlipyTrendingCacheWorker> _logger,
    IKlipyTrendingCache _klipyTrendingCache,
    IOptionsMonitor<AppConfig> _appConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await Task.Delay(_appConfig.CurrentValue.Klipy.Staging.TrendingCaching.TimeSpanBetweenRefreshes, stoppingToken);

                _logger.LogKlipyTrendingCacheIsRefreshing();

                await _klipyTrendingCache.RefreshTrendingGifsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogTopLevelException(exception);
            }
            finally
            {
                _logger.LogKlipyTrendingCacheHasRefreshed();
            }
    }
}
