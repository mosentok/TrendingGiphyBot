using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class ToggleModalOpeningInteractionModule
(
    IChannelSettingsDtoBuilder _dtoBuilder,
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory
) : BothHooksDtoInteractionModuleBase(_dtoBuilder)
{
    [ComponentInteraction(InteractionId.HowOftenOpenButton)]
    public Task OpenHowOftenAsync()
    {
        Component = _settingsMessageComponentFactory.BuildHowOftenToggleModal(ChannelSettingsDto, Context.Channel.Name);

        return Task.CompletedTask;
    }

    [ComponentInteraction(InteractionId.PostingBehaviorOpenButton)]
    public Task OpenPostingBehaviorAsync()
    {
        Component = _settingsMessageComponentFactory.BuildPostingBehaviorToggleModal(ChannelSettingsDto, Context.Channel.Name);

        return Task.CompletedTask;
    }

    [ComponentInteraction(InteractionId.GifSourcesOpenButton)]
    public Task OpenGifSourcesAsync()
    {
        Component = _settingsMessageComponentFactory.BuildGifSourcesToggleModal(ChannelSettingsDto, Context.Channel.Name);

        return Task.CompletedTask;
    }

    [ComponentInteraction(InteractionId.GifRetentionOpenButton)]
    public Task OpenGifRetentionAsync()
    {
        Component = _settingsMessageComponentFactory.BuildGifRetentionToggleModal(ChannelSettingsDto, Context.Channel.Name);

        return Task.CompletedTask;
    }

    [ComponentInteraction(InteractionId.GiphyRatingOpenButton)]
    public Task OpenGiphyRatingAsync()
    {
        Component = _settingsMessageComponentFactory.BuildGiphyRatingToggleModal(ChannelSettingsDto, Context.Channel.Name);

        return Task.CompletedTask;
    }
}
