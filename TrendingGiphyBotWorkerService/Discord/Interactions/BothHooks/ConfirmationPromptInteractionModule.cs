using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class ConfirmationPromptInteractionModule
(
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory,
    IPendingConfirmationManager _confirmationManager
) : ComponentUpdateInteractionModuleBase
{
    [ComponentInteraction(InteractionId.ResetHowOftenButton)]
    public async Task ResetHowOftenAsync()
    {
        await _confirmationManager.SetPendingAsync(
            Context.Channel.Id,
            PendingClearAction.HowOften,
            "Are you sure you want to reset how often the bot posts gifs to 30 minutes?"
        );

        Component = _settingsMessageComponentFactory.BuildConfirmationModal("Are you sure you want to reset how often the bot posts gifs to 30 minutes?", Context.Channel.Name);
    }

    [ComponentInteraction(InteractionId.ResetPostingBehaviorButton)]
    public async Task ResetPostingBehaviorAsync()
    {
        await _confirmationManager.SetPendingAsync(
            Context.Channel.Id,
            PendingClearAction.PostingBehavior,
            "Are you sure you want to reset the posting behavior to Trending Gifs Only?"
        );

        Component = _settingsMessageComponentFactory.BuildConfirmationModal("Are you sure you want to reset the posting behavior to Trending Gifs Only?", Context.Channel.Name);
    }

    [ComponentInteraction(InteractionId.ClearGifSourcesButton)]
    public async Task ClearGifSourcesAsync()
    {
        await _confirmationManager.SetPendingAsync(
            Context.Channel.Id,
            PendingClearAction.GifSources,
            "Are you sure you want to clear gif sources?"
        );

        Component = _settingsMessageComponentFactory.BuildConfirmationModal("Are you sure you want to clear gif sources?", Context.Channel.Name);
    }

    [ComponentInteraction(InteractionId.ResetRetentionPeriodButton)]
    public async Task ResetGifRetentionAsync()
    {
        await _confirmationManager.SetPendingAsync(
            Context.Channel.Id,
            PendingClearAction.GifRetention,
            "Are you sure you want to reset the retention period to 14 days?"
        );

        Component = _settingsMessageComponentFactory.BuildConfirmationModal("Are you sure you want to reset the retention period to 14 days?", Context.Channel.Name);
    }

    [ComponentInteraction(InteractionId.ResetGiphyRatingButton)]
    public async Task ResetGiphyRatingAsync()
    {
        await _confirmationManager.SetPendingAsync(
            Context.Channel.Id,
            PendingClearAction.GiphyRating,
            "Are you sure you want to reset the giphy rating to PG?"
        );

        Component = _settingsMessageComponentFactory.BuildConfirmationModal("Are you sure you want to reset the giphy rating to PG?", Context.Channel.Name);
    }

    [ComponentInteraction(InteractionId.ClearKeywordButton)]
    public async Task ClearKeywordAsync()
    {
        await _confirmationManager.SetPendingAsync(
            Context.Channel.Id,
            PendingClearAction.Keyword,
            "Are you sure you want to clear random gif keywords?"
        );

        Component = _settingsMessageComponentFactory.BuildConfirmationModal("Are you sure you want to clear random gif keywords?", Context.Channel.Name);
    }

    [ComponentInteraction(InteractionId.ClearPostingHoursButton)]
    public async Task ClearPostingHoursAsync()
    {
        await _confirmationManager.SetPendingAsync(
            Context.Channel.Id,
            PendingClearAction.PostingHours,
            "Are you sure you want to clear posting hours?"
        );

        Component = _settingsMessageComponentFactory.BuildConfirmationModal("Are you sure you want to clear posting hours?", Context.Channel.Name);
    }
}
