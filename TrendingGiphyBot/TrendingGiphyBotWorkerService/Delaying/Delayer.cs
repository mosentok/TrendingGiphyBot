namespace TrendingGiphyBotWorkerService.Delaying;

public class Delayer(TimeProvider _timeProvider, DelayerConfig _delayerConfig) : IDelayer
{
    public async Task DelayUntilNextPostingTimeAsync(CancellationToken cancellationToken)
    {
        var utcNow = _timeProvider.GetUtcNow();
        var utcNext = _delayerConfig.CronExpression.GetNextOccurrence(utcNow.UtcDateTime);

        if (!utcNext.HasValue)
            throw new ThisShouldBeImpossibleException();

        var delay = utcNext.Value - utcNow.UtcDateTime;

        if (delay > TimeSpan.Zero)
            await Task.Delay(delay, cancellationToken);
    }
}
