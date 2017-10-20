using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using NLog;

namespace TrendingGiphyBot.Modules
{
    [Group("SetPrefix")]
    public class SetPrefixModule : LoggingModuleBase
    {
        public SetPrefixModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()){}
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help() => await HelpMessageReplyAsync();
        [Command(nameof(Get))]
        public async Task Get()
        {
            if (await GlobalConfig.ChannelConfigDal.Any(Context.Channel.Id))
            {
                var prefix = await GlobalConfig.ChannelConfigDal.GetPrefix(Context.Channel.Id);
                await ReplyAsync($"Your prefix is {prefix}");
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
                if (await GlobalConfig.ChannelConfigDal.Any(Context.Channel.Id))
                    await GlobalConfig.ChannelConfigDal.SetPrefix(Context.Channel.Id, prefix);
                else
                    await GlobalConfig.ChannelConfigDal.Insert(Context.Channel.Id, prefix);
                await Get();
            }
            else
                await ReplyAsync("Prefix must be 1-4 characters long.");
        }
        [Command(nameof(Reset))]
        public async Task Reset()
        {
            await Set(GlobalConfig.Config.DefaultPrefix);
        }
    }
}
