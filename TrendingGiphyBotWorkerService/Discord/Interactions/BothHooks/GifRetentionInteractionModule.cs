using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class GifRetentionInteractionModule
(
    ITrendingGiphyBotDbContext trendingGiphyBotContext,
    IChannelSettingsDtoBuilder dtoBuilder,
    IChannelSettingsInteractionUpdater interactionUpdater
) : BothHooksInteractionModuleBase(trendingGiphyBotContext, dtoBuilder, interactionUpdater)
{
    [ComponentInteraction(InteractionId.GifRetentionButtonWildcard)]
    public async Task SelectGifRetentionAsync(string gifRetention)
    {
        ChannelSettingsModel.RetentionDays = int.Parse(gifRetention);

        await TrendingGiphyBotContext.SaveChangesAsync();
    }
}
