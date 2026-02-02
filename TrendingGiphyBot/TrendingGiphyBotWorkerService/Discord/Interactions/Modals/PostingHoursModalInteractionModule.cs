using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions;
using TrendingGiphyBotWorkerService.Utc;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.Modals;

public class PostingHoursModalInteractionModule(
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory,
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IUtcOffsetParser _utcOffsetParser
) : InteractionModuleBase<SocketInteractionContext<SocketModal>>
{
    const string _utcOffsetErrorMessage = "Please input your time zone UTC offset in the format '+ab:xy' or '-ab:xy', like -03:00, +05:30, or 1245.";

    [ModalInteraction(InteractionId.TrendingPostingHoursModal)]
    public async Task SetPostingHoursAsync(PostingHoursModal postingHoursModal)
    {
        var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

        if (postingHoursModal.From is not null or "")
        {
            var fromSuccess = int.TryParse(postingHoursModal.From, out var from);

            if (!fromSuccess)
                throw new ThisShouldBeImpossibleException();

            channelSettings.PostingHoursFrom = from;
        }

        if (postingHoursModal.To is not null or "")
        {
            var toSuccess = int.TryParse(postingHoursModal.To, out var to);

            if (!toSuccess)
                throw new ThisShouldBeImpossibleException();

            channelSettings.PostingHoursTo = to;
        }

        if (postingHoursModal.UtcOffset is not null or "")
        {
            if (postingHoursModal.UtcOffset is not string { Length: 6 } utcOffsetString)
            {
                await Context.Interaction.FollowupAsync(_utcOffsetErrorMessage);

                return;
            }

            var (success, utcOffset) = await _utcOffsetParser.TryParseUtcOffsetAsync(utcOffsetString);

            if (!success || utcOffset is null)
            {
                await Context.Interaction.FollowupAsync(_utcOffsetErrorMessage);

                return;
            }

            channelSettings.UtcOffset = utcOffset.ToString();
        }

        await _trendingGiphyBotContext.SaveChangesAsync();

        var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(channelSettings, Context.Channel.Name);

        await Context.Interaction.UpdateAsync(async messageProperties => messageProperties.Components = settingsMessageComponent);
    }
}
