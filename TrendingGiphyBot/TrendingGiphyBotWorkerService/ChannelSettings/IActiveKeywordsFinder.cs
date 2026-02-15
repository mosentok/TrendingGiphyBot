namespace TrendingGiphyBotWorkerService.ChannelSettings;

public interface IActiveKeywordsFinder
{
    Task<string[]> FindActiveKeywordsAsync(CancellationToken cancellationToken = default);
}
