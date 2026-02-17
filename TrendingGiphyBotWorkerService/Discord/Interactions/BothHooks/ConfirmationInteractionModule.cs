using Discord.Interactions;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;

public class ConfirmationInteractionModule
(
    ITrendingGiphyBotDbContext trendingGiphyBotContext,
    IChannelSettingsDtoBuilder dtoBuilder,
    IChannelSettingsInteractionUpdater interactionUpdater,
    IPendingConfirmationManager _confirmationManager
) : BothHooksInteractionModuleBase(trendingGiphyBotContext, dtoBuilder, interactionUpdater)
{

    [ComponentInteraction(InteractionId.ConfirmClearButton)]
    public async Task ConfirmClearAsync()
    {
        var found = await _confirmationManager.TryGetAndRemoveAsync(Context.Channel.Id, out var confirmation);

        if (!found)
            return;

        switch (confirmation.Action)
        {
            case PendingClearAction.HowOften:
                ChannelSettingsModel.Frequency = 30;
                ChannelSettingsModel.IntervalId = (int)IntervalDescription.Minutes;

                break;

            case PendingClearAction.PostingBehavior:
                ChannelSettingsModel.GifPostingBehaviorId = (int)GifPostingBehaviorKind.TrendingGifsOnly;

                break;

            case PendingClearAction.GifSources:
                ChannelSettingsModel.GifSource = GifSourceKind.None;

                break;

            case PendingClearAction.GifRetention:
                ChannelSettingsModel.RetentionDays = 14;

                break;

            case PendingClearAction.GiphyRating:
                ChannelSettingsModel.GiphyRating = "pg";

                break;

            case PendingClearAction.Keyword:
                ChannelSettingsModel.GifKeyword = null;

                break;

            case PendingClearAction.PostingHours:
                ChannelSettingsModel.PostingHoursFrom = null;
                ChannelSettingsModel.PostingHoursTo = null;
                ChannelSettingsModel.UtcOffset = null;

                break;
        }

        await TrendingGiphyBotContext.SaveChangesAsync();
    }
}
