using System.Threading.Tasks;
using Discord.Commands;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Helpers;
using TrendingGiphyBot.Jobs;
using System.Linq;
using Discord;
using TrendingGiphyBot.Attributes;
using System;
using TrendingGiphyBot.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(JobConfig))]
    public class JobConfigModule : ModuleBase
    {
        IServiceProvider _Services;
        IGlobalConfig _GlobalConfig;
        public JobConfigModule(IServiceProvider services)
        {
            _Services = services;
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        string NotConfiguredMessage => $"{Context.Channel.Id} not configured. Configure me senpai! Use '!{nameof(JobConfig)}' or '!{nameof(JobConfig)} {nameof(Help)}' to learn how to.";
        [Command(nameof(Help))]
        [Summary("Help menu for the " + nameof(JobConfig) + " commands.")]
        [Alias(nameof(Help))]
        public async Task Help()
        {
            var avatarUrl = Context.Client.CurrentUser.GetAvatarUrl();
            var author = new EmbedAuthorBuilder()
                .WithName(nameof(JobConfig))
                .WithIconUrl(avatarUrl);
            var fields = ModuleBaseHelper.BuildFields<JobConfigModule>();
            var embed = new EmbedBuilder { Fields = fields }
                .WithAuthor(author)
                .WithDescription($"Commands for interacting with {nameof(JobConfig)}.");
            await ReplyAsync(string.Empty, embed: embed);
        }
        [Command(nameof(Get))]
        [Summary("Gets the " + nameof(JobConfig) + " for this channel.")]
        public async Task Get()
        {
            var any = await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id);
            if (any)
            {
                var config = await _GlobalConfig.JobConfigDal.Get(Context.Channel.Id);
                var avatarUrl = (await Context.Client.GetGuildAsync(Context.Guild.Id)).IconUrl;
                var author = new EmbedAuthorBuilder()
                    .WithName($"{Context.Channel.Name}'s {nameof(JobConfig)}")
                    .WithIconUrl(avatarUrl);
                var embed = new EmbedBuilder()
                    .WithAuthor(author)
                    .AddInlineField(nameof(config.Interval), config.Interval)
                    .AddInlineField(nameof(config.Time), config.Time);
                await ReplyAsync(string.Empty, embed: embed);
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        [Command(nameof(Set))]
        [Summary("Sets the " + nameof(JobConfig) + " for this channel.")]
        [Example("!jobconfig set 10 minutes")]
        public async Task Set(
            [Summary(nameof(JobConfig.Interval) + " to set.")]
            int interval,
            [Summary(nameof(JobConfig.Time) + " to set.")]
            Time time)
        {
            var isAtLeastMinInterval = ModuleBaseHelper.IsAtLeastMinInterval(interval, time, _GlobalConfig.Config.MinimumMinutes);
            if (isAtLeastMinInterval)
            {
                var state = ModuleBaseHelper.DetermineJobConfigState(interval, time);
                switch (state)
                {
                    case JobConfigState.InvalidHours:
                        await ReplyAsync($"When {nameof(Time)} is {time}, interval must be {ModuleBaseHelper.ValidHoursString}.");
                        return;
                    case JobConfigState.InvalidMinutes:
                    case JobConfigState.InvalidSeconds:
                        await ReplyAsync($"When {nameof(Time)} is {time}, interval must be {ModuleBaseHelper.ValidMinutesSecondsString}.");
                        return;
                    case JobConfigState.InvalidTime:
                        await ReplyAsync($"{time} is an invalid {nameof(Time)}.");
                        return;
                    case JobConfigState.Valid:
                        await SaveConfig(interval, time);
                        await Get();
                        return;
                }
            }
            else
                await ReplyAsync($"{nameof(JobConfig.Interval)} and {nameof(JobConfig.Time)} must combine to at least {_GlobalConfig.Config.MinimumMinutes} minutes.");
        }
        [Command(nameof(Remove))]
        [Summary("Removes the " + nameof(JobConfig) + " for this channel.")]
        public async Task Remove()
        {
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                await SendRemoveMessage();
                await _GlobalConfig.JobConfigDal.Remove(Context.Channel.Id);
                var toRemove = _GlobalConfig.Jobs.OfType<PostImageJob>().Single(s => s.ChannelId == Context.Channel.Id);
                _GlobalConfig.Jobs.Remove(toRemove);
                toRemove?.Dispose();
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        async Task SendRemoveMessage()
        {
            if (_GlobalConfig.Config.UseWordnik)
            {
                var wordOfTheDay = await _GlobalConfig.WordnikClient.GetWordOfTheDay();
                await ReplyAsync($"Configuration removed. {CapitalizeFirstLetter(wordOfTheDay.Word)}.");
            }
            else
                await ReplyAsync("Configuration removed.");
        }
        static string CapitalizeFirstLetter(string s) => char.ToUpper(s[0]) + s.Substring(1);
        async Task SaveConfig(int interval, Time time)
        {
            var config = new JobConfig
            {
                ChannelId = Context.Channel.Id,
                Interval = interval,
                Time = time.ToString()
            };
            var any = await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id);
            if (any)
            {
                await _GlobalConfig.JobConfigDal.Update(config);
                _GlobalConfig.Jobs.OfType<PostImageJob>().Single(s => s.ChannelId == Context.Channel.Id).Restart(config);
            }
            else
            {
                await _GlobalConfig.JobConfigDal.Insert(config);
                var postImageJob = new PostImageJob(_Services, config);
                _GlobalConfig.Jobs.Add(postImageJob);
                postImageJob.StartTimerWithCloseInterval();
            }
        }
    }
}
