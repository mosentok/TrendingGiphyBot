using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrendingGiphyBot.Attributes;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Helpers;
using TrendingGiphyBot.Jobs;

namespace TrendingGiphyBot.Modules
{
    [Group(_Name)]
    public class SetRandomModule : ModuleBase
    {
        const string _Name = "SetRandom";
        IServiceProvider _Services;
        IGlobalConfig _GlobalConfig;
        public SetRandomModule(IServiceProvider services)
        {
            _Services = services;
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        string NotConfiguredMessage => $"{Context.Channel.Id} not configured. Configure me senpai! Please set {nameof(JobConfig)} first. Use '!{nameof(JobConfig)}' or '!{nameof(JobConfig)} {nameof(Help)}' to learn how to.";
        [Command(nameof(Help))]
        [Summary("Help menu for the " + _Name + " commands.")]
        [Alias(nameof(Help), "")]
        [Example("!" + _Name + " " + nameof(Help))]
        public async Task Help()
        {
            var avatarUrl = Context.Client.CurrentUser.GetAvatarUrl();
            var author = new EmbedAuthorBuilder()
                .WithName(_Name)
                .WithIconUrl(avatarUrl);
            var fields = ModuleBaseHelper.BuildFields<SetRandomModule>();
            var embed = new EmbedBuilder { Fields = fields }
                .WithAuthor(author)
                .WithDescription($"Commands for interacting with {_Name}.\n- Want the bot to post a random GIF if there's no new trending one?");
            await ReplyAsync(string.Empty, embed: embed);
        }
        [Command(nameof(Get))]
        [Summary("Gets the random option for this channel.")]
        [Example("!" + _Name + " " + nameof(Get))]
        public async Task Get()
        {
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var any = await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id);
                if (any)
                {
                    var config = await _GlobalConfig.JobConfigDal.Get(Context.Channel.Id);
                    var avatarUrl = (await Context.Client.GetGuildAsync(Context.Guild.Id)).IconUrl;
                    var author = new EmbedAuthorBuilder()
                        .WithName($"{Context.Channel.Name}'s {_Name}")
                        .WithIconUrl(avatarUrl);
                    EmbedBuilder embedBuilder = new EmbedBuilder()
                        .WithAuthor(author)
                        .AddInlineField(nameof(config.RandomIsOn), config.RandomIsOn);
                    EmbedBuilder embed;
                    if (!string.IsNullOrEmpty(config.RandomSearchString))
                        embed = embedBuilder
                            .AddInlineField(nameof(config.RandomSearchString), config.RandomSearchString);
                    else
                        embed = embedBuilder
                            .AddInlineField(nameof(config.RandomSearchString), "null");
                    await ReplyAsync(string.Empty, embed: embed);
                }
                else
                    await ReplyAsync(NotConfiguredMessage);
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        [Command(nameof(On))]
        [Summary("Sets the random option for this channel.")]
        [Example("!SetRandom " + nameof(On), "!setrandom on cats")]
        public async Task On(
            [IsOptional]
            [Summary("Text to search for when getting a random GIF.")]
            params string[] searchString)
        {
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var config = new JobConfig
                {
                    ChannelId = Context.Channel.Id,
                    RandomIsOn = true,
                    RandomSearchString = string.Join(" ", searchString)
                };
                await UpdateJobs(config);
                await _GlobalConfig.JobConfigDal.UpdateRandom(config);
                await Get();
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        [Command(nameof(Off))]
        [Summary("Removes the random option for this channel.")]
        [Example("!" + _Name + " " + nameof(Off))]
        public async Task Off()
        {
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var config = new JobConfig { ChannelId = Context.Channel.Id, RandomIsOn = false };
                await UpdateJobs(config);
                await _GlobalConfig.JobConfigDal.UpdateRandom(config);
                await ReplyAsync("No more Randies.");
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        Task UpdateJobs(JobConfig config)
        {
            var postImageJobs = _GlobalConfig.Jobs.OfType<PostImageJob>().ToList();
            var jobConfig = postImageJobs.SelectMany(s => s.JobConfigs).Single(s => s.ChannelId == Context.Channel.Id);
            jobConfig.RandomIsOn = config.RandomIsOn;
            jobConfig.RandomSearchString = config.RandomSearchString;
            return Task.CompletedTask;
        }
        static string CapitalizeFirstLetter(string s) => char.ToUpper(s[0]) + s.Substring(1);
    }
}
