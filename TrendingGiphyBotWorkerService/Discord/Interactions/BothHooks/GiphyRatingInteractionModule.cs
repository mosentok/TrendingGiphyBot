using Discord;
using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class GiphyRatingInteractionModule
(
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IChannelSettingsDtoBuilder _dtoBuilder,
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory
) : BothHooksToggleInteractionModuleBase(_trendingGiphyBotContext, _dtoBuilder, _settingsMessageComponentFactory)
{
    [ComponentInteraction(InteractionId.GiphyRatingGButton)]
    public async Task SelectGiphyRatingGAsync()
    {
        ChannelSettingsModel.GiphyRating = "g";

        await TrendingGiphyBotContext.SaveChangesAsync();
    }

    [ComponentInteraction(InteractionId.GiphyRatingPgButton)]
    public async Task SelectGiphyRatingPgAsync()
    {
        ChannelSettingsModel.GiphyRating = "pg";

        await TrendingGiphyBotContext.SaveChangesAsync();
    }

    [ComponentInteraction(InteractionId.GiphyRatingPg13Button)]
    public async Task SelectGiphyRatingPg13Async()
    {
        ChannelSettingsModel.GiphyRating = "pg-13";

        await TrendingGiphyBotContext.SaveChangesAsync();
    }

    [ComponentInteraction(InteractionId.GiphyRatingRButton)]
    public async Task SelectGiphyRatingRAsync()
    {
        ChannelSettingsModel.GiphyRating = "r";

        await TrendingGiphyBotContext.SaveChangesAsync();
    }

    [ComponentInteraction(InteractionId.GiphyRatingAllButton)]
    public async Task SelectGiphyRatingAllAsync()
    {
        ChannelSettingsModel.GiphyRating = "all";

        await TrendingGiphyBotContext.SaveChangesAsync();
    }

    protected override MessageComponent BuildToggleModal(ChannelSettingsDto channelSettings, string channelName) =>
        SettingsMessageComponentFactory.BuildGiphyRatingToggleModal(channelSettings, channelName);
}
