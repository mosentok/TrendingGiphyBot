using Discord;
using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class GiphyRatingInteractionModule
(
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IChannelSettingsDtoBuilder _dtoBuilder,
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory
) : BothHooksToggleInteractionModuleBase(_trendingGiphyBotContext, _dtoBuilder, _settingsMessageComponentFactory)
{
    [ComponentInteraction(InteractionId.GiphyRatingGButton)]
    public async Task SelectGiphyRatingGAsync() =>
        ChannelSettingsModel.GiphyRating = "g";

    [ComponentInteraction(InteractionId.GiphyRatingPgButton)]
    public async Task SelectGiphyRatingPgAsync() =>
        ChannelSettingsModel.GiphyRating = "pg";

    [ComponentInteraction(InteractionId.GiphyRatingPg13Button)]
    public async Task SelectGiphyRatingPg13Async() =>
        ChannelSettingsModel.GiphyRating = "pg-13";

    [ComponentInteraction(InteractionId.GiphyRatingRButton)]
    public async Task SelectGiphyRatingRAsync() =>
        ChannelSettingsModel.GiphyRating = "r";

    [ComponentInteraction(InteractionId.GiphyRatingAllButton)]
    public async Task SelectGiphyRatingAllAsync() =>
        ChannelSettingsModel.GiphyRating = "all";

    protected override MessageComponent BuildToggleModal(ChannelSettingsDto channelSettings, string channelName) =>
        SettingsMessageComponentFactory.BuildGiphyRatingToggleModal(channelSettings, channelName);
}
