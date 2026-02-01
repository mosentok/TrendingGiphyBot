using Cronos;

namespace TrendingGiphyBotWorkerService.Delaying;

public record DelayerConfig(string CronExpressionString)
{
    CronExpression? cronExpression;

    public CronExpression CronExpression
    {
        get
        {
            if (cronExpression is null)
                cronExpression = CronExpression.Parse(CronExpressionString);

            return cronExpression;
        }
    }
}