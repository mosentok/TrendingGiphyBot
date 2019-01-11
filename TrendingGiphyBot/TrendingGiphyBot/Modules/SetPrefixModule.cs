using Discord.Commands;
using System;
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
        public async Task Help() => await SendDeprecatedCommandMessage();
        [Command(nameof(Get))]
        public async Task Get() => await SendDeprecatedCommandMessage();
        [Command(nameof(Set))]
        public async Task Set(string prefix) => await SendDeprecatedCommandMessage();
        [Command(nameof(Reset))]
        public async Task Reset() => await SendDeprecatedCommandMessage();
    }
}
