using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using TrendingGiphyBot.Attributes;

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
            var avatarUrl = Context.Client.CurrentUser.GetAvatarUrl();
            var author = new EmbedAuthorBuilder()
                .WithName(nameof(TrendingGiphyBot))
                .WithIconUrl(avatarUrl);
            var type = GetType();
            var fields = ModuleBaseHelper.BuildFields<MainModule>();
            var embed = new EmbedBuilder { Fields = fields }
                .WithAuthor(author)
                .WithDescription($"Commands for interacting with {nameof(TrendingGiphyBot)}.");
            await ReplyAsync(string.Empty, embed: embed);
        }
        [ExcludeAttribute]
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
