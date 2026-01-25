using Discord.Interactions;

namespace TrendingGiphyBotWorkerService.Interactions.Modals;

public class PostingHoursModal : IModal
{
    public string Title => "Set the hours during which you want the bot to post.";

    [InputLabel("From")]
    [ModalTextInput(InteractionId.TrendingPostingHoursFromTextInput)]
    public string? From { get; set; }

    [InputLabel("To")]
    [ModalTextInput(InteractionId.TrendingPostingHoursToTextInput)]
    public string? To { get; set; }

    [InputLabel("Time Zone UTC Offset")]
    [ModalTextInput(InteractionId.TrendingPostingHoursUtcOffsetTextInput)]
    public string? UtcOffset { get; set; }
}