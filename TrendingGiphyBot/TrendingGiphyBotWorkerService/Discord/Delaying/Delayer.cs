using Microsoft.Extensions.Options;

namespace TrendingGiphyBotWorkerService.Discord.Delaying;

[RegisterSingleton]
public class Delayer(TimeProvider _timeProvider, IOptions<AppConfig> _appConfig) : IDelayer
{
    public async Task DelayUntilNextPostingTimeAsync(CancellationToken cancellationToken)
    {
        var utcNow = _timeProvider.GetUtcNow();
        var utcNext = _appConfig.Value.Delayer.CronExpression.GetNextOccurrence(utcNow.UtcDateTime);

        if (!utcNext.HasValue)
            throw new ThisShouldBeImpossibleException();

        var delay = utcNext.Value - utcNow.UtcDateTime;

        if (delay > TimeSpan.Zero)
            await Task.Delay(delay, cancellationToken);
    }
}
