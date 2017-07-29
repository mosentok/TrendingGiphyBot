using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrendingGiphyBot.Configuration;

namespace TrendingGiphyBot.Modules
{
    [Group("SetPrefix")]
    public class SetPrefixModule : ModuleBase
    {
        readonly IGlobalConfig _GlobalConfig;
        public SetPrefixModule(IServiceProvider services)
        {
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help()
        {
            await ReplyAsync($"Visit {_GlobalConfig.Config.GitHubUrl} for help!");
        }
        [Command(nameof(Get))]
        public async Task Get()
        {
            if (await _GlobalConfig.ChannelConfigDal.Any(Context.Channel.Id))
            {
                var prefix = await _GlobalConfig.ChannelConfigDal.GetPrefix(Context.Channel.Id);
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
                if (await _GlobalConfig.ChannelConfigDal.Any(Context.Channel.Id))
                    await _GlobalConfig.ChannelConfigDal.SetPrefix(Context.Channel.Id, prefix);
                else
                    await _GlobalConfig.ChannelConfigDal.Insert(Context.Channel.Id, prefix);
                await Get();
            }
            else
                await ReplyAsync("Prefix must be 1-4 characters long.");
        }
        [Command(nameof(Reset))]
        public async Task Reset()
        {
            await Set(_GlobalConfig.Config.DefaultPrefix);
        }
    }
}
