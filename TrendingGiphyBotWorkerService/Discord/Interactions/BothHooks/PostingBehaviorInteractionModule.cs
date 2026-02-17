using Discord;
using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions;
using TrendingGiphyBotWorkerService.GifPostingBehavior;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class PostingBehaviorInteractionModule
(
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IChannelSettingsDtoBuilder _dtoBuilder,
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory,
    IGifPostingBehaviorHelper _gifPostingBehaviorHelper
) : BothHooksToggleInteractionModuleBase(_trendingGiphyBotContext, _dtoBuilder, _settingsMessageComponentFactory)
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

    protected override MessageComponent BuildToggleModal(ChannelSettingsDto channelSettings, string channelName) =>
        SettingsMessageComponentFactory.BuildPostingBehaviorToggleModal(channelSettings, channelName);
}
