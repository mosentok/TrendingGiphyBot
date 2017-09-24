using System.Threading.Tasks;
using NLog;

namespace TrendingGiphyBot
{
    static class Program
    {
        static async Task Main()
        {
            await LogManager.GetCurrentClassLogger().SwallowAsync(async () =>
            {
                using (var bot = new Bot())
                {
                    await bot.Run();
                    await Task.Delay(-1);
                }
            });
        }
    }
}
