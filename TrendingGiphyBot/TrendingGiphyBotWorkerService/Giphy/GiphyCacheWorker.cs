using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.Giphy;

public class GiphyCacheWorker(ILoggerWrapper<GiphyCacheWorker> _loggerWrapper, IGiphyClient _giphyClient, IGifCache _gifCache, int _maxPageCount, TimeSpan _timeBetweenRefreshes) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var numberOfResponses = 0;

                while (numberOfResponses % _maxPageCount == 0)
                {
                    var giphyResponse = await _giphyClient.GetTrendingGifsAsync(offset: numberOfResponses, cancellationToken: stoppingToken);

                    _gifCache.Add(giphyResponse.Data);

                    numberOfResponses += giphyResponse.Data.Count;
                }

                await Task.Delay(_timeBetweenRefreshes, stoppingToken);
            }
        }
        catch (Exception exception)
        {
            _loggerWrapper.LogTopLevelException(exception);
        }
    }
}
