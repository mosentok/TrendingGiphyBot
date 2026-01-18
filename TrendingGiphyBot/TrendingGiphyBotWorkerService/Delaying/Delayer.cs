namespace TrendingGiphyBotWorkerService.Delaying;

public class Delayer(TimeProvider _timeProvider, DelayerConfig _delayerConfig) : IDelayer
{
    public async Task DelayUntilNextPostingTimeAsync(CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow();
        var next = _delayerConfig.CronExpression.GetNextOccurrence(now.UtcDateTime);

        if (!next.HasValue)
            throw new ThisShouldBeImpossibleException();

        var delay = next.Value - now.UtcDateTime;

        if (delay > TimeSpan.Zero)
            await Task.Delay(delay, cancellationToken);
    }
}
