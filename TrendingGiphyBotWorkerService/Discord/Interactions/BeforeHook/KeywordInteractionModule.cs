using Discord;
using Discord.Interactions;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.BeforeHook;

public class KeywordInteractionModule(ITrendingGiphyBotDbContext _trendingGiphyBotContext) : BeforeHookModelInteractionModuleBase(_trendingGiphyBotContext)
{
    [ComponentInteraction(InteractionId.TrendingGifsWithKeywordButton)]
    public async Task OpenKeywordModalAsync()
    {
        var gifKeywordModal = new ModalBuilder()
            .WithTitle("Set keywords to post gifs of when up-to-date")
            .WithCustomId(InteractionId.TrendingGifsWithKeywordModal)
            .AddTextInput("Keywords", InteractionId.TrendingGifsWithKeywordTextInput, placeholder: "cats", required: true, value: ChannelSettingsModel.GifKeyword)
            .Build();

        await Context.Interaction.RespondWithModalAsync(gifKeywordModal);
    }
}
