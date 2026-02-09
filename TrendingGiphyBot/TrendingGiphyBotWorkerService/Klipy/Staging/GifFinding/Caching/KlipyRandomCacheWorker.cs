using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching;

public class KlipyRandomCacheWorker(
    ILogger<KlipyRandomCacheWorker> _logger,
    IKlipyRandomCache _klipyRandomCache,
    IOptionsMonitor<AppConfig> _appConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await Task.Delay(_appConfig.CurrentValue.Klipy.Staging.RandomCaching.TimeSpanBetweenRefreshes, stoppingToken);

                _logger.LogKlipyRandomCacheIsRefreshing();

                await _klipyRandomCache.RefreshRandomGifsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogTopLevelException(exception);
            }
            finally
            {
                _logger.LogKlipyRandomCacheHasRefreshed();
            }
    }
}
