namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

public interface IHowOftenFormatter
{
    string Format(int frequency, int intervalId);
}
