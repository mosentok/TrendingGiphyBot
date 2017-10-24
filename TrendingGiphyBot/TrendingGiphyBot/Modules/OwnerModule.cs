using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using NLog;
using TrendingGiphyBot.Extensions;

namespace TrendingGiphyBot.Modules
{
    [Group("Owner")]
    public class OwnerModule : BotModuleBase
    {
        public OwnerModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()) { }
        [Command(nameof(RefreshConfig))]
        public async Task RefreshConfig()
        {
            if (Context.User.Id == GlobalConfig.Config.OwnerId)
            {
                await GlobalConfig.RefreshConfig();
                await TryReplyAsync("Done.");
            }
        }
        [Command(nameof(SetGame))]
        public async Task SetGame(params string[] literalSentenceToSetAsGame)
        {
            if (Context.User.Id == GlobalConfig.Config.OwnerId)
            {
                var game = DetermineGame(literalSentenceToSetAsGame);
                if (!string.IsNullOrEmpty(game))
                {
                    await GlobalConfig.DiscordClient.SetGameAsync(game);
                    await TryReplyAsync("Done.");
                }
            }
        }
        static string DetermineGame(string[] words)
        {
            if (words.Any())
                return words.FlattenWith(" ");
            return null;
        }
    }
}
