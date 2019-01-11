using System.Threading.Tasks;
using Discord.Commands;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Dals;
using System;
using NLog;

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(JobConfig))]
    public class JobConfigModule : BotModuleBase
    {
        public JobConfigModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()) { }
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help() => await SendDeprecatedCommandMessage();
        [Command(nameof(Get))]
        public async Task Get() => await SendDeprecatedCommandMessage();
        [Command(nameof(Set))]
        public async Task Set(int interval, Time time) => await SendDeprecatedCommandMessage();
        [Command(nameof(Remove))]
        public async Task Remove() => await SendDeprecatedCommandMessage();
    }
}
