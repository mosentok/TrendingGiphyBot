using Discord.Interactions;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.Modals;

public class KeywordModal : IModal
{
    public string Title => "unused";

    [ModalTextInput(InteractionId.TrendingGifsWithKeywordTextInput)]
    public required string Keyword { get; set; }
}
