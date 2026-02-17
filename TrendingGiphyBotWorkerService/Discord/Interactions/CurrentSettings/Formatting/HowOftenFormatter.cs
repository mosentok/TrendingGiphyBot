using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

[RegisterSingleton]
public class HowOftenFormatter : IHowOftenFormatter
{
    public string Format(int frequency, IntervalDescription interval)
    {
        if (interval == IntervalDescription.None || frequency == 0)
            return "Never";

        var unit = interval switch
        {
            IntervalDescription.Minutes => frequency == 1 ? "Minute" : "Minutes",
            IntervalDescription.Hours => frequency == 1 ? "Hour" : "Hours",
            _ => ""
        };

        return $"{frequency} {unit}";
    }
}
