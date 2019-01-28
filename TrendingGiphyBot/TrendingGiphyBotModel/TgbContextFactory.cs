using Microsoft.EntityFrameworkCore;

namespace TrendingGiphyBotModel
{
    public class TgbContextFactory : DesignTimeDbContextFactoryBase<TrendingGiphyBotContext>
    {
        protected override TrendingGiphyBotContext CreateNewInstance(DbContextOptions<TrendingGiphyBotContext> options)
        {
            return new TrendingGiphyBotContext(options);
        }
    }
}