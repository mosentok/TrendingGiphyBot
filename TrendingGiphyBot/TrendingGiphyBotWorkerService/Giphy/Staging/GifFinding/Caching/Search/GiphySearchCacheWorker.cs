using Microsoft.Extensions.Options;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy.Staging.GifFinding.Caching.Search;

public class GiphySearchCacheWorker
(
    ILogger<GiphySearchCacheWorker> _logger,
    IGiphySearchCacheRefresher _giphySearchCacheRefresher,
    IOptionsMonitor<AppConfig> _appConfig
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await Task.Delay(_appConfig.CurrentValue.Giphy.Staging.SearchCaching.TimeSpanBetweenRefreshes, stoppingToken);

                _logger.LogGiphySearchCacheIsRefreshing();

                await _giphySearchCacheRefresher.RefreshSearchCachesForActiveKeywordsAsync(stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogTopLevelException(exception);
            }
            finally
            {
                _logger.LogGiphySearchCacheHasRefreshed();
            }
    }
}
