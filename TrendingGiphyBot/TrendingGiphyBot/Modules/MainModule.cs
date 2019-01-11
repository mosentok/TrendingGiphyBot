using Discord.Commands;
using System;
using System.Threading.Tasks;
using NLog;

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(TrendingGiphyBot))]
    [Alias("TGB")]
    public class MainModule : BotModuleBase
    {
        public MainModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()){}
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help() => await SendDeprecatedCommandMessage();
    }
}
