using Cronos;

namespace TrendingGiphyBotWorkerService.Discord.GifPosting.Delaying;

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