using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Interactions;

public class PostingHoursButtonInteractionModule(ITrendingGiphyBotDbContext _trendingGiphyBotContext, IUtcOffsetParser _utcOffsetParser) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("trending-posting-hours-modal-button")]
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
            .WithTitle("Set keyword to post gifs of when up-to-date")
            .WithCustomId("trending-posting-hours-modal")
            .AddTextInput("From (24 hour time)", "trending-posting-hours-from-text-input", placeholder: "10", maxLength: 2, required: false, value: channelSettings.PostingHoursFrom?.ToString() ?? string.Empty)
            .AddTextInput("To (24 hour time)", "trending-posting-hours-to-text-input", placeholder: "22", maxLength: 2, required: false, value: channelSettings.PostingHoursTo?.ToString() ?? string.Empty)
            // TODO string format the decimal
            .AddTextInput("Time Zone UTC Offset (+/-ab:xy)", "trending-posting-hours-utc-offset-text-input", placeholder: "-12:00, -3:30, +6:00", maxLength: 6, required: false, value: formattedUtcOffset)
            .Build();

        await Context.Interaction.RespondWithModalAsync(postingHooursModal);
    }
}