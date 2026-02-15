using Discord.Interactions;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.Modals;

public class KeyboardModal : IModal
{
    public string Title => "Set keywords to post gifs of when up-to-date";

    [ModalTextInput(InteractionId.TrendingGifsWithKeywordTextInput)]
    public required string Keyword { get; set; }
}
