using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Helpers;
using Microsoft.Extensions.DependencyInjection;
using TrendingGiphyBot.Dals;
using System.Collections.Generic;

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(TrendingGiphyBot))]
    [Alias("TGB")]
    public class MainModule : ModuleBase
    {
        readonly IGlobalConfig _GlobalConfig;
        public MainModule(IServiceProvider services)
        {
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        [Command(nameof(Help))]
        [Summary("Help menu for the " + nameof(TrendingGiphyBot) + " commands.")]
        [Alias(nameof(Help), "")]
        public async Task Help()
        {
            await ReplyAsync($"Visit {_GlobalConfig.Config.GitHubUrl} for help!");
            //await SendHelpMenu();
        }
        async Task SendHelpMenu()
        {
            var avatarUrl = Context.Client.CurrentUser.GetAvatarUrl();
            var author = new EmbedAuthorBuilder()
                .WithName(nameof(TrendingGiphyBot))
                .WithIconUrl(avatarUrl);
            var type = GetType();
            var fields = ModuleBaseHelper.BuildFields<MainModule>();
            AddModuleField(fields, nameof(JobConfig));
            AddModuleField(fields, "SetRandom");
            var embed = new EmbedBuilder { Fields = fields }
                .WithAuthor(author)
                .WithDescription($"Commands for interacting with {nameof(TrendingGiphyBot)}.\n- Check out the [docs]({_GlobalConfig.Config.GitHubUrl})!")
                .WithUrl(_GlobalConfig.Config.GitHubUrl);
            await ReplyAsync(string.Empty, embed: embed);
        }
        static void AddModuleField(List<EmbedFieldBuilder> fields, string moduleName)
        {
            fields.Add(new EmbedFieldBuilder()
                .WithName(moduleName)
                .WithValue($"Check !{moduleName}"));
        }
    }
}
