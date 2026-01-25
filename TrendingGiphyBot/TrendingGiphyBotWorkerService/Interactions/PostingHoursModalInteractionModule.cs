using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Interactions;

public class PostingHoursModalInteractionModule(
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory,
    ITrendingGiphyBotDbContext _trendingGiphyBotContext
) : InteractionModuleBase<SocketInteractionContext<SocketModal>>
{
    const string _utcOffsetErrorMessage = "Please input your time zone UTC offset in the format '+ab:xy' or '+ab:xy', like -03:00, +05:30, or 1245.";

    [ModalInteraction("trending-posting-hours-modal")]
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

            var (success, utcOffset) = await TryParseUtcOffsetAsync(utcOffsetString);

            if (!success || utcOffset is null)
            {
                await Context.Interaction.FollowupAsync(_utcOffsetErrorMessage);

                return;
            }

            channelSettings.UtcOffset = utcOffset;
        }

        await _trendingGiphyBotContext.SaveChangesAsync();

        var settingsMessageComponent = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(channelSettings, Context.Channel.Name);

        await Context.Interaction.UpdateAsync(async messageProperties => messageProperties.Components = settingsMessageComponent);
    }

    public async Task<(bool Success, decimal? UtcOffset)> TryParseUtcOffsetAsync(string stringWithSign)
    {
        short sign;

        switch (stringWithSign[0])
        {
            case '+':
                sign = 1;
                break;
            case '-':
                sign = -1;
                break;
            default:
                await Context.Interaction.FollowupAsync(_utcOffsetErrorMessage);

                return (false, null);
        }

        var split = stringWithSign[1..].Split([':'], 2);

        if (split.Length != 2)
            return (false, null);

        var hoursSuccess = !int.TryParse(split[0], out var hours);

        if (hoursSuccess)
            return (false, null);

        var minutesSuccess = !int.TryParse(split[1], out var minutes);

        if (minutesSuccess)
            return (false, null);

        if (hours < 0 || hours > 14 || minutes < 0 || minutes > 59)
            return (false, null);

        var decimalOffset = hours + (minutes / 100m);

        return (true, decimalOffset * sign);
    }
}
