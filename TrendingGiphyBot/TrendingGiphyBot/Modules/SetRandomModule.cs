using Discord.Commands;
using System;
using System.Threading.Tasks;
using NLog;

namespace TrendingGiphyBot.Modules
{
    [Group("SetRandom")]
    public class SetRandomModule : BotModuleBase
    {
        public SetRandomModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()) { }
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help() => await SendDeprecatedCommandMessage();
        [Command(nameof(Get))]
        public async Task Get() => await SendDeprecatedCommandMessage();
        [Command(nameof(On))]
        public async Task On(params string[] searchValues) => await SendDeprecatedCommandMessage();
        [Command(nameof(Off))]
        public async Task Off() => await SendDeprecatedCommandMessage();
    }
}
