using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using TrendingGiphyBot.Attributes;
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
        [ExcludeThis]
        [Command(nameof(Dev))]
        [Summary("Help menu for the " + nameof(Dev) + " commands.")]
        [Alias(nameof(Dev))]
        public async Task Dev()
        {
            var playback = _GlobalConfig.SpotifyClient.GetPlayingTrack();
            //TODO centralize the avatar url access
            var avatarUrl = Context.Client.CurrentUser.GetAvatarUrl();
            var author = new EmbedAuthorBuilder()
                .WithName(nameof(TrendingGiphyBot))
                .WithIconUrl(avatarUrl);
            var artists = string.Join(", ", playback.Item.Artists.Select(s => s.Name));
            var embed = new EmbedBuilder()
                .WithAuthor(author)
                .WithDescription($"The developer is listening to [{playback.Item.Name}]({playback.Item.Album.Uri}) by [{artists}]({playback.Item.Uri}).");
            if (playback.Item.Album.Images.Any())
                embed = embed.WithImageUrl(playback.Item.Album.Images.First().Url);
            await ReplyAsync(string.Empty, embed: embed);
        }
    }
}
