using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Intervals;

public class IntervalSeeder(IServiceScopeFactory _serviceScopeFactory) : IIntervalSeeder
{
    public async Task SeedIntervalsAsync()
    {
        var thereAreNewChangesToSave = false;

        var expectedIntervals = Enum.GetValues<IntervalDescription>().Select(intervalDescription => new Interval
        {
            IntervalId = (int)intervalDescription,
            Description = intervalDescription.ToString()
        });

        using var scope = _serviceScopeFactory.CreateScope();

        var _trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();

        var intervals = await _trendingGiphyBotDbContext.Intervals.ToDictionaryAsync(s => s.IntervalId, s => s);

        foreach (var expectedInterval in expectedIntervals)
        {
            var intervalAlreadyExists = intervals.ContainsKey(expectedInterval.IntervalId);

            if (!intervalAlreadyExists)
            {
                _trendingGiphyBotDbContext.Intervals.Add(expectedInterval);

                thereAreNewChangesToSave = true;
            }
            else if (intervals[expectedInterval.IntervalId].Description != expectedInterval.Description)
            {
                intervals[expectedInterval.IntervalId].Description = expectedInterval.Description;

                thereAreNewChangesToSave = true;
            }
        }

        if (thereAreNewChangesToSave)
            await _trendingGiphyBotDbContext.SaveChangesAsync();
    }
}