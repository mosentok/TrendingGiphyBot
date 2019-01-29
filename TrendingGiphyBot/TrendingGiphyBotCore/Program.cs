using System.Threading.Tasks;

namespace TrendingGiphyBotCore
{
    public static class Program
    {
        public static async Task Main()
        {
            while (true)
                using (var bot = new Bot())
                {
                    await bot.Run();
                    await Task.Delay(-1);
                }
        }
    }
}
