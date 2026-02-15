using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.Database;

namespace TrendingGiphyBotWorkerService.Intervals;

[RegisterSingleton]
public class IntervalSeeder(
    ILogger<IntervalSeeder> _logger,
    IServiceScopeFactory _serviceScopeFactory
) : IIntervalSeeder
{
    public async Task SeedIntervalsAsync()
    {
        _logger.LogSeedingIntervals();

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
                _trendingGiphyBotDbContext.Intervals.Add(expectedInterval);
            else
                intervals[expectedInterval.IntervalId].Description = expectedInterval.Description;
        }

        await _trendingGiphyBotDbContext.SaveChangesAsync();

        _logger.LogSeededIntervals();
    }
}