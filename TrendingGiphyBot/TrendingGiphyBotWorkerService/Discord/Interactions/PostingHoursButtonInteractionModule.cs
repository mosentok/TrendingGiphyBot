using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;
using TrendingGiphyBotWorkerService.Utc;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public class PostingHoursButtonInteractionModule(ITrendingGiphyBotDbContext _trendingGiphyBotContext, IUtcOffsetParser _utcOffsetParser) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction(InteractionId.TrendingPostingHoursModalButton)]
    public async Task OpenKeywordModalAsync()
    {
        var channelSettings = await _trendingGiphyBotContext.ChannelSettings
            .Where(s => s.ChannelId == Context.Channel.Id)
            .Select(s => new { s.PostingHoursFrom, s.PostingHoursTo, s.UtcOffset })
            .SingleAsync();

        var formattedUtcOffset = channelSettings.UtcOffset is null or ""
            ? string.Empty
            : _utcOffsetParser.FormatUtcOffsetString(channelSettings.UtcOffset);

        var postingHooursModal = new ModalBuilder()
            .WithTitle("Set the hours and timezone in which the bot should post")
            .WithCustomId(InteractionId.TrendingPostingHoursModal)
            .AddTextInput("From (24 hour time)", InteractionId.TrendingPostingHoursFromTextInput, placeholder: "10", maxLength: 2, required: false, value: channelSettings.PostingHoursFrom?.ToString() ?? string.Empty)
            .AddTextInput("To (24 hour time)", InteractionId.TrendingPostingHoursToTextInput, placeholder: "22", maxLength: 2, required: false, value: channelSettings.PostingHoursTo?.ToString() ?? string.Empty)
            .AddTextInput("Time Zone UTC Offset (+/-ab:xy)", InteractionId.TrendingPostingHoursUtcOffsetTextInput, placeholder: "-12:00, -3:30, +6:00", maxLength: 6, required: false, value: formattedUtcOffset)
            .Build();

        await Context.Interaction.RespondWithModalAsync(postingHooursModal);
    }
}