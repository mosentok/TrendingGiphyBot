using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Discord.Interactions.BothHooks;
using TrendingGiphyBotWorkerService.Utc;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.Modals;

public class PostingHoursModalInteractionModule(
    IChannelSettingsMessageComponentFactory _settingsMessageComponentFactory,
    ITrendingGiphyBotDbContext _trendingGiphyBotContext,
    IUtcOffsetParser _utcOffsetParser,
    IChannelSettingsDtoBuilder _dtoBuilder
) : BothHooksModalInteractionModuleBase
{
    const string _utcOffsetErrorMessage = "Please input your time zone UTC offset in the format '+ab:xy' or '-ab:xy', like -03:00, +05:30, or 1245.";

    [ModalInteraction(InteractionId.TrendingPostingHoursModal)]
    public async Task SetPostingHoursAsync(PostingHoursModal postingHoursModal)
    {
        var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

        var from = postingHoursModal.From is null or ""
            ? new int?()
            : int.Parse(postingHoursModal.From);

        var to = postingHoursModal.To is null or ""
            ? new int?()
            : int.Parse(postingHoursModal.To);

        var utcOffsetValue = ParseUtcOffset(postingHoursModal.UtcOffset);

        channelSettings.PostingHours = new(from, to, utcOffsetValue);

        await _trendingGiphyBotContext.SaveChangesAsync();

        var dto = await _dtoBuilder.BuildFromChannelIdAsync(Context.Channel.Id);

        Component = _settingsMessageComponentFactory.BuildChannelSettingsMessageComponent(dto, Context.Channel.Name);

        string? ParseUtcOffset(string? utcOffsetValue)
        {
            if (utcOffsetValue is null or "")
                return null;

            if (utcOffsetValue is not string { Length: 6 } utcOffsetString)
                throw new InvalidOperationException(_utcOffsetErrorMessage);

            var (success, utcOffset) = _utcOffsetParser.TryParseUtcOffset(utcOffsetString);

            if (!success || utcOffset is null)
                throw new InvalidOperationException(_utcOffsetErrorMessage);

            return utcOffset.ToString();
        }
    }
}
