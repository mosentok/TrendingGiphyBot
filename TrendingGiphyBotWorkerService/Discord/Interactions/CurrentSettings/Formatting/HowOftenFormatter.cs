using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

[RegisterSingleton]
public class HowOftenFormatter : IHowOftenFormatter
{
    public string Format(int frequency, int intervalId)
    {
        var intervalDescription = (IntervalDescription)intervalId;

        if (intervalDescription == IntervalDescription.None || frequency == 0)
            return "Never";

        var unit = intervalDescription switch
        {
            IntervalDescription.Minutes => frequency == 1 ? "Minute" : "Minutes",
            IntervalDescription.Hours => frequency == 1 ? "Hour" : "Hours",
            _ => ""
        };

        return $"{frequency} {unit}";
    }
}
