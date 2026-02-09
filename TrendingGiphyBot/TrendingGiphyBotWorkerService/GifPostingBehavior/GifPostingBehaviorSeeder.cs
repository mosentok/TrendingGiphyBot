using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Logging;

namespace TrendingGiphyBotWorkerService.GifPostingBehavior;

[RegisterSingleton]
public class GifPostingBehaviorSeeder(
    ILogger<GifPostingBehaviorSeeder> _logger,
    IServiceScopeFactory _serviceScopeFactory
) : IGifPostingBehaviorSeeder
{
    public async Task SeedGifPostingBehaviorsAsync()
    {
        _logger.LogSeedingGifPostingBehaviors();

        var expectedGifPostingBehaviors = Enum.GetValues<GifPostingBehaviorKind>().Select(gifPostingBehavior => new GifPostingBehaviorModel
        {
            GifPostingBehaviorId = (int)gifPostingBehavior,
            Description = gifPostingBehavior.ToString()
        });

        using var scope = _serviceScopeFactory.CreateScope();

        var _trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var gifPostingBehaviorModels = await _trendingGiphyBotDbContext.GifPostingBehaviors.ToDictionaryAsync(s => s.GifPostingBehaviorId, s => s);

        foreach (var expectedGifPostingBehavior in expectedGifPostingBehaviors)
        {
            var gifPostingBehaviorAlreadyExists = gifPostingBehaviorModels.ContainsKey(expectedGifPostingBehavior.GifPostingBehaviorId);

            if (!gifPostingBehaviorAlreadyExists)
                _trendingGiphyBotDbContext.GifPostingBehaviors.Add(expectedGifPostingBehavior);
            else
                gifPostingBehaviorModels[expectedGifPostingBehavior.GifPostingBehaviorId].Description = expectedGifPostingBehavior.Description;
        }

        await _trendingGiphyBotDbContext.SaveChangesAsync();

        _logger.LogSeededGifPostingBehaviors();
    }
}
