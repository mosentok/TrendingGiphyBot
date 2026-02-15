using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Klipy.Staging.GifFinding.Caching.Search;

public class KlipySearchCacheWorker
(
    ILogger<KlipySearchCacheWorker> _logger,
    IKlipySearchCacheRefresher _klipySearchCacheRefresher,
    IOptionsMonitor<AppConfig> _appConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await Task.Delay(_appConfig.CurrentValue.Klipy.Staging.SearchCaching.TimeSpanBetweenRefreshes, stoppingToken);

                _logger.LogKlipySearchCacheIsRefreshing();

                await _klipySearchCacheRefresher.RefreshSearchCachesForActiveKeywordsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogTopLevelException(exception);
            }
            finally
            {
                _logger.LogKlipySearchCacheHasRefreshed();
            }
    }
}
