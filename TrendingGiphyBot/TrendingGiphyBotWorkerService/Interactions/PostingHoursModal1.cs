using Discord.Interactions;

namespace TrendingGiphyBotWorkerService.Interactions;

public class PostingHoursModal : IModal
{
    public string Title => "Set the hours during which you want the bot to post.";

    [InputLabel("From")]
    [ModalTextInput("trending-posting-hours-from-text-input", placeholder: "cats")]
    public string? From { get; set; }

    [InputLabel("To")]
    [ModalTextInput("trending-posting-hours-to-text-input", placeholder: "cats")]
    public string? To { get; set; }

    [InputLabel("Time Zone UTC Offset")]
    [ModalTextInput("trending-posting-hours-utc-offset-text-input", placeholder: "-12:00, -03:00, TODO MORE")]
    public string? UtcOffset { get; set; }
}