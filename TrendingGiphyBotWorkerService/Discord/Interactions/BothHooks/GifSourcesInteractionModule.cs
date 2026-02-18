using Discord;
using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class GifSourcesInteractionModule
(
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IChannelSettingsDtoBuilder _dtoBuilder,
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory
) : BothHooksToggleInteractionModuleBase(_trendingGiphyBotContext, _dtoBuilder, _settingsMessageComponentFactory)
{

    [ComponentInteraction(InteractionId.GifSourceGiphyButton)]
    public async Task ToggleGifSourceGiphyAsync()
    {
        ChannelSettingsModel.GifSource = ChannelSettingsModel.GifSource.HasFlag(GifSourceKind.Giphy)
            ? ChannelSettingsModel.GifSource &= ~GifSourceKind.Giphy
            : ChannelSettingsModel.GifSource |= GifSourceKind.Giphy;

        await TrendingGiphyBotContext.SaveChangesAsync();
    }

    [ComponentInteraction(InteractionId.GifSourceKlipyButton)]
    public async Task ToggleGifSourceKlipyAsync()
    {
        ChannelSettingsModel.GifSource = ChannelSettingsModel.GifSource.HasFlag(GifSourceKind.Klipy)
            ? ChannelSettingsModel.GifSource &= ~GifSourceKind.Klipy
            : ChannelSettingsModel.GifSource |= GifSourceKind.Klipy;

        await TrendingGiphyBotContext.SaveChangesAsync();
    }

    protected override MessageComponent BuildToggleModal(ChannelSettingsDto channelSettings, string channelName) =>
        SettingsMessageComponentFactory.BuildGifSourcesToggleModal(channelSettings, channelName);
}
