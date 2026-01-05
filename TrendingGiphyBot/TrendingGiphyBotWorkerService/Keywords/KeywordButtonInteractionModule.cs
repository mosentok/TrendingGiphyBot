using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Keywords;

public class KeywordButtonInteractionModule(ITrendingGiphyBotContext _trendingGiphyBotContext) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction("trending-gifs-with-keyword-modal-button")]
    public async Task OpenKeywordModalAsync()
    {
        var gifKeyword = await _trendingGiphyBotContext.ChannelSettings.Where(s => s.ChannelId == Context.Channel.Id).Select(s => s.GifKeyword).SingleAsync();

        var gifKeywordModal = new ModalBuilder()
            .WithTitle("Set keyword to post gifs of when up-to-date")
            .WithCustomId("trending-gifs-with-keyword-modal")
            .AddTextInput("Keyword", "trending-gifs-with-keyword-text-input", placeholder: "cats", required: true, value: gifKeyword)
            .Build();

        await Context.Interaction.RespondWithModalAsync(gifKeywordModal);
    }
}