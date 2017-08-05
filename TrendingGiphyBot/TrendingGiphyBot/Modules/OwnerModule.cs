using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using NLog;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Modules
{
    [Group("Owner")]
    public class OwnerModule : LoggingModuleBase
    {
        public OwnerModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()) { }
        [Command(nameof(RefreshConfig))]
        public async Task RefreshConfig()
        {
            if (Context.User.Id == GlobalConfig.Config.OwnerId)
            {
                await GlobalConfig.RefreshConfig();
                await ReplyAsync("Done.");
            }
        }
        [Command(nameof(SetGame))]
        public async Task SetGame(params string[] searchValues)
        {
            if (Context.User.Id == GlobalConfig.Config.OwnerId)
            {
                var game = DetermineGame(searchValues);
                if (!string.IsNullOrEmpty(game))
                {
                    await GlobalConfig.DiscordClient.SetGameAsync(game);
                    await ReplyAsync("Done.");
                }
            }
        }
        static string DetermineGame(string[] searchValues)
        {
            if (searchValues.Any())
                return searchValues.Join(" ");
            return null;
        }
    }
}
