using Discord.Interactions;

namespace TrendingGiphyBotWorkerService.Interactions;

public class KeyboardModal : IModal
{
    public string Title => "Set keyword to post gifs of when up-to-date";

    [InputLabel("Keyword")]
    [ModalTextInput("trending-gifs-with-keyword-text-input", placeholder: "cats")]
    public required string Keyword { get; set; }
}
