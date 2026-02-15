using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

[RegisterSingleton]
public class ActiveKeywordsFinder(IServiceScopeFactory _serviceScopeFactory) : IActiveKeywordsFinder
{
    public async Task<string[]> FindActiveKeywordsAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        return await dbContext.ChannelSettings
            .Where(s => s.Frequency > 0 && s.GifKeyword != null && s.GifKeyword != "")
            .Select(s => s.GifKeyword!)
            .Distinct()
            .ToArrayAsync(cancellationToken);
    }
}
