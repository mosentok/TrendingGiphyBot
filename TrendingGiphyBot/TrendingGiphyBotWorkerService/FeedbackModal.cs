using Discord.Interactions;

namespace TrendingGiphyBotWorkerService
{
    public class FeedbackModal : IModal
    {
        public string Title => "Set keyword to post gifs of when up-to-date";

        [InputLabel("Keyword")]
        [ModalTextInput("trending-gifs-with-keyword-text-input", placeholder: "cats")]
        public required string Keyword { get; set; }
    }
}