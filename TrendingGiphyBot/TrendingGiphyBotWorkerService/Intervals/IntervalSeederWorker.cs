using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Intervals;

public class IntervalSeederWorker(IServiceScopeFactory _serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var thereAreNewChangesToSave = false;
        var expectedIntervals = Enum.GetValues<IntervalDescription>().Select(intervalDescription => new Interval
        {
            IntervalId = (int)intervalDescription,
            Description = intervalDescription.ToString()
        });
        using var scope = _serviceScopeFactory.CreateScope();

        var _trendingGiphyBotDbContext = scope.ServiceProvider.GetRequiredService<ITrendingGiphyBotDbContext>();
		var intervals = await _trendingGiphyBotDbContext.Intervals.ToListAsync(stoppingToken);

        foreach (var expectedInterval in expectedIntervals)
        {
            var existingInterval = intervals.SingleOrDefault(s => s.IntervalId == expectedInterval.IntervalId);

            if (existingInterval is null)
            {
                _trendingGiphyBotDbContext.Intervals.Add(expectedInterval);

                thereAreNewChangesToSave = true;
            }
            else if (existingInterval.Description != expectedInterval.Description)
            {
                existingInterval.Description = expectedInterval.Description;

                thereAreNewChangesToSave = true;
            }
        }

        if (thereAreNewChangesToSave)
            await _trendingGiphyBotDbContext.SaveChangesAsync(stoppingToken);
    }
}