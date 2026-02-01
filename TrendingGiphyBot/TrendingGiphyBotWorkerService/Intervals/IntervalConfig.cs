using System.Text.Json;
using TrendingGiphyBotWorkerService.Configuration;

namespace TrendingGiphyBotWorkerService.Intervals;

public record IntervalConfig(string MinutesJsonArrayString, string HoursJsonArrayString)
{
    int[]? minutes;

    public int[] Minutes
    {
        get
        {
            if (MinutesJsonArrayString is null)
                throw new MissingConfigurationException();

            if (minutes is null)
                minutes = JsonSerializer.Deserialize<int[]>(MinutesJsonArrayString) ?? throw new MissingConfigurationException();

            return minutes;
        }
    }

    int[]? hours;

    public int[] Hours
    {
        get
        {
            if (HoursJsonArrayString is null)
                throw new MissingConfigurationException();

            if (hours is null)
                hours = JsonSerializer.Deserialize<int[]>(HoursJsonArrayString) ?? throw new MissingConfigurationException();

            return hours;
        }
    }
}