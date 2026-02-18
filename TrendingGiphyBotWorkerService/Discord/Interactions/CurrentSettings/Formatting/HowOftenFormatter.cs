using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Discord.Interactions.CurrentSettings.Formatting;

[RegisterSingleton]
public class HowOftenFormatter : IHowOftenFormatter
{
    public string Format(int frequency, Interval interval) =>
        new { interval, frequency } switch
        {
            { interval: Interval.None } or { frequency: 0 } => "Never",
            { interval: Interval.Minutes, frequency: 1 } => "1 Minute",
            { interval: Interval.Minutes, frequency: not 1 } => $"{frequency} Minutes",
            { interval: Interval.Hours, frequency: 1 } => "1 Hour",
            { interval: Interval.Hours, frequency: not 1 } => $"{frequency} Hours",
            _ => throw new ThisShouldBeImpossibleException()
        };
}
