using Discord.Interactions;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class PostingBehaviorInteractionModule
(
    ITrendingGiphyBotDbContext trendingGiphyBotContext,
    IChannelSettingsDtoBuilder dtoBuilder,
    IChannelSettingsInteractionUpdater interactionUpdater,
    IGifPostingBehaviorHelper _gifPostingBehaviorHelper
) : BothHooksInteractionModuleBase(trendingGiphyBotContext, dtoBuilder, interactionUpdater)
{

    [ComponentInteraction(InteractionId.PostingBehaviorTrendingOnlyButton)]
    public async Task SelectPostingBehaviorTrendingOnlyAsync()
    {
        await _gifPostingBehaviorHelper.SetBehaviorAsync(ChannelSettingsModel, GifPostingBehaviorKind.TrendingGifsOnly);

        await TrendingGiphyBotContext.SaveChangesAsync();
    }

    [ComponentInteraction(InteractionId.PostingBehaviorTrendingWithRandomButton)]
    public async Task SelectPostingBehaviorTrendingWithRandomAsync()
    {
        await _gifPostingBehaviorHelper.SetBehaviorAsync(ChannelSettingsModel, GifPostingBehaviorKind.TrendingGifsWithRandomGifs);

        await TrendingGiphyBotContext.SaveChangesAsync();
    }
}
