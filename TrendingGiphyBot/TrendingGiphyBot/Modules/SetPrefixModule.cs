using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using TrendingGiphyBot.Configuration;

namespace TrendingGiphyBot.Modules
{
    [Group("SetPrefix")]
    public class SetPrefixModule : ModuleBase
    {
        IServiceProvider _Services;
        IGlobalConfig _GlobalConfig;
        public SetPrefixModule(IServiceProvider services)
        {
            _Services = services;
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        [Command(nameof(Set))]
        public async void Set(string prefix)
        {
            var isValid = !string.IsNullOrEmpty(prefix) && prefix.Any() && prefix.Length <= 4;
            if (isValid)
                if (await _GlobalConfig.ChannelConfigDal.Any(Context.Channel.Id))
                    await _GlobalConfig.ChannelConfigDal.SetPrefix(Context.Channel.Id, prefix);
                else
                    await _GlobalConfig.ChannelConfigDal.Insert(Context.Channel.Id, prefix);
            else
                await ReplyAsync("Prefix must be 1-4 characters long.");
        }
        public void Reset()
        {

        }
    }
}
