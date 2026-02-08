using Cronos;

namespace TrendingGiphyBotWorkerService.Discord.Delaying;

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