namespace TrendingGiphyBotWorkerService.Intervals;

public static class IntervalDescriptionExtensions
{
    extension(IntervalDescription intervalDescription)
    {
        public string GetDescription() =>
            intervalDescription switch
            {
                IntervalDescription.None => "None",
                IntervalDescription.Minutes => "Minutes",
                IntervalDescription.Hours => "Hours",
                _ => throw new ThisShouldBeImpossibleException()
            };
    }
}
