using System.Text;
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
    [ModalInteraction(InteractionId.TrendingPostingHoursModal)]
    public async Task SetPostingHoursAsync(PostingHoursModal postingHoursModal)
    {
        var channelSettings = await _trendingGiphyBotContext.ChannelSettings.SingleAsync(s => s.ChannelId == Context.Channel.Id);

        var errorMessagesBuilder = new StringBuilder();

        var fromSuccess = int.TryParse(postingHoursModal.From, out var from);

        if (!fromSuccess || from is not (> 1 and < 24))
        {
            var fromError = postingHoursModal.From is null or ""
                ? "<blank>"
                : postingHoursModal.From;

            errorMessagesBuilder.AppendLine($"From must be between 1 and 24. Input: {fromError}");
        }

        var toSuccess = int.TryParse(postingHoursModal.To, out var to);

        if (!toSuccess || from is not (> 1 and < 24))
        {
            var toError = postingHoursModal.To is null or ""
                ? "<blank>"
                : postingHoursModal.To;

            errorMessagesBuilder.AppendLine($"To must be between 1 and 24. Input: {toError}");
        }

        var errorMessages = errorMessagesBuilder.ToString();

        if (errorMessages is not (null or ""))
        {
            var errorMessage = new StringBuilder()
                .AppendLine("The input was invalid. Please try again.")
                .AppendLine()
                .Append(errorMessages)
                .ToString();

            await Context.Interaction.RespondAsync(errorMessage, ephemeral: true);

            return;
        }

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
                return AddErrorMessage();

            var (success, utcOffset) = _utcOffsetParser.TryParseUtcOffset(utcOffsetString);

            if (!success || utcOffset is null)
                return AddErrorMessage();

            return utcOffset.ToString();

            string? AddErrorMessage()
            {
                var utcOffsetError = utcOffsetValue is null or ""
                    ? "<blank>"
                    : utcOffsetValue;

                errorMessagesBuilder.AppendLine($"""Please input your time zone UTC offset in the format "+ab:xy" or "-ab:xy", like -03:00, +05:30, or +12:45. Input: {utcOffsetError}""");

                return null;
            }
        }
    }
}
