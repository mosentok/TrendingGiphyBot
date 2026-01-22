using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.GifPostingBehavior;

public class GifPostingBehaviorSeeder(IServiceScopeFactory _serviceScopeFactory) : IGifPostingBehaviorSeeder
{
    public async Task SeedGifPostingBehaviorsAsync()
    {
        var thereAreNewChangesToSave = false;

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
            {
                _trendingGiphyBotDbContext.GifPostingBehaviors.Add(expectedGifPostingBehavior);

                thereAreNewChangesToSave = true;
            }
            else if (gifPostingBehaviorModels[expectedGifPostingBehavior.GifPostingBehaviorId].Description != expectedGifPostingBehavior.Description)
            {
                gifPostingBehaviorModels[expectedGifPostingBehavior.GifPostingBehaviorId].Description = expectedGifPostingBehavior.Description;

                thereAreNewChangesToSave = true;
            }
        }

        if (thereAreNewChangesToSave)
            await _trendingGiphyBotDbContext.SaveChangesAsync();
    }
}
