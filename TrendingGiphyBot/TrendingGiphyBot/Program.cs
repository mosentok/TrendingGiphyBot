using System.Data.Entity;
using System.Threading.Tasks;
using NLog;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot
{
    static class Program
    {
        static async Task Main()
        {
            await LogManager.GetCurrentClassLogger().SwallowAsync(async () =>
            {
                DbConfiguration.SetConfiguration(new SqlAzureConfiguration());
                using (var bot = new Bot())
                {
                    await bot.Run();
                    await Task.Delay(-1);
                }
            });
        }
    }
}
