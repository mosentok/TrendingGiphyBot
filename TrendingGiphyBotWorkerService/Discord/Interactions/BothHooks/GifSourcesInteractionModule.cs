using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class GifSourcesInteractionModule
(
    ITrendingGiphyBotDbContext trendingGiphyBotContext,
    IChannelSettingsDtoBuilder dtoBuilder,
    IChannelSettingsInteractionUpdater interactionUpdater
) : BothHooksInteractionModuleBase(trendingGiphyBotContext, dtoBuilder, interactionUpdater)
{

    [ComponentInteraction(InteractionId.GifSourceGiphyButton)]
    public async Task ToggleGifSourceGiphyAsync()
    {
        var sources = ChannelSettingsModel.GifSource ?? (GifSourceKind.Giphy | GifSourceKind.Klipy);

        if (sources.HasFlag(GifSourceKind.Giphy))
            sources &= ~GifSourceKind.Giphy;
        else
            sources |= GifSourceKind.Giphy;

        ChannelSettingsModel.GifSource = sources;

        await TrendingGiphyBotContext.SaveChangesAsync();
    }

    [ComponentInteraction(InteractionId.GifSourceKlipyButton)]
    public async Task ToggleGifSourceKlipyAsync()
    {
        var sources = ChannelSettingsModel.GifSource ?? (GifSourceKind.Giphy | GifSourceKind.Klipy);

        if (sources.HasFlag(GifSourceKind.Klipy))
            sources &= ~GifSourceKind.Klipy;
        else
            sources |= GifSourceKind.Klipy;

        ChannelSettingsModel.GifSource = sources;

        await TrendingGiphyBotContext.SaveChangesAsync();
    }
}
