namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

[RegisterSingleton]
public class PostingHoursFormatter : IPostingHoursFormatter
{
    public string Format(int? postingHoursFrom, int? postingHoursTo)
    {
        if (postingHoursFrom is null || postingHoursTo is null)
            return "Any Time";

        return $"{postingHoursFrom:D2}:00 - {postingHoursTo:D2}:00";
    }
}
