using Discord.Interactions;

namespace TrendingGiphyBotWorkerService.Interactions.Modals;

public class KeyboardModal : IModal
{
    public string Title => "Set keyword to post gifs of when up-to-date";

    [InputLabel("Keyword")]
    [ModalTextInput(InteractionId.TrendingGifsWithKeywordTextInput, placeholder: "cats")]
    public required string Keyword { get; set; }
}
