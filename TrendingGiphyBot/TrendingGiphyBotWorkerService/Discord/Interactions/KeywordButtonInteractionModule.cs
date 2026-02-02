using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Discord.Interactions;

public class KeywordButtonInteractionModule(ITrendingGiphyBotDbContext _trendingGiphyBotContext) : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    [ComponentInteraction(InteractionId.TrendingGifsWithKeywordModalButton)]
    public async Task OpenKeywordModalAsync()
    {
        var gifKeyword = await _trendingGiphyBotContext.ChannelSettings.Where(s => s.ChannelId == Context.Channel.Id).Select(s => s.GifKeyword).SingleAsync();

        var gifKeywordModal = new ModalBuilder()
            .WithTitle("Set keyword to post gifs of when up-to-date")
            .WithCustomId(InteractionId.TrendingGifsWithKeywordModal)
            .AddTextInput("Keyword", InteractionId.TrendingGifsWithKeywordTextInput, placeholder: "cats", required: true, value: gifKeyword)
            .Build();

        await Context.Interaction.RespondWithModalAsync(gifKeywordModal);
    }
}
