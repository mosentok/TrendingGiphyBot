using Discord.Interactions;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.Modals;

public class PostingHoursModal : IModal
{
    public string Title => "unused";

    [ModalTextInput(InteractionId.TrendingPostingHoursFromTextInput)]
    public string? From { get; set; }

    [ModalTextInput(InteractionId.TrendingPostingHoursToTextInput)]
    public string? To { get; set; }

    [ModalTextInput(InteractionId.TrendingPostingHoursUtcOffsetTextInput)]
    public string? UtcOffset { get; set; }
}