namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

public interface IPostingHoursFormatter
{
    string Format(int? postingHoursFrom, int? postingHoursTo);
}
