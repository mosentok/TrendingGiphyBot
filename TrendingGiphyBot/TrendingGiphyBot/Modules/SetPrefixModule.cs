using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace TrendingGiphyBot.Modules
{
    [Group("SetPrefix")]
    public class SetPrefixModule : BotModuleBase
    {
        public SetPrefixModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()) { }
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help() => await HelpMessageReplyAsync();
        [Command(nameof(Get))]
        public async Task Get()
        {
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                if (await entities.AnyChannelConfigs(Context.Channel.Id))
                {
                    var prefix = await entities.GetPrefix(Context.Channel.Id);
                    await TryReplyAsync($"Your prefix is {prefix}");
                }
                else
                    await Reset();
        }
        [Command(nameof(Set))]
        public async Task Set(string prefix)
        {
            var isValid = !string.IsNullOrEmpty(prefix) && prefix.Any() && prefix.Length <= 4;
            if (isValid)
            {
                using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                    if (await entities.AnyChannelConfigs(Context.Channel.Id))
                        await entities.SetPrefix(Context.Channel.Id, prefix);
                    else
                        await entities.InsertChannelConfig(Context.Channel.Id, prefix);
                await Get();
            }
            else
                await TryReplyAsync("Prefix must be 1-4 characters long.");
        }
        [Command(nameof(Reset))]
        public async Task Reset()
        {
            await Set(GlobalConfig.Config.DefaultPrefix);
        }
    }
}
