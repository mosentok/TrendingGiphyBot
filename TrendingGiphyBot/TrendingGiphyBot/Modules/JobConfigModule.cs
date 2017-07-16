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
        [Alias(nameof(Help), "")]
        [Example("!" + nameof(JobConfig) + " " + nameof(Help))]
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
        [Example("!" + nameof(JobConfig) + " " + nameof(Get))]
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
        [Example("!JobConfig Set 5 Seconds", "!jobconfig set 10 minutes", "!JoBcOnFiG sEt 1 HoUr")]
        public async Task Set(
            [Summary(nameof(JobConfig.Interval) + " to set.")]
            int interval,
            [Summary(nameof(JobConfig.Time) + " to set.")]
            Time time)
        {
            var state = Job.DetermineJobConfigState(interval, time, _GlobalConfig.Config.MinimumSeconds, _GlobalConfig.Config.MaximumSeconds);
            switch (state)
            {
                case JobConfigState.InvalidHours:
                    await ReplyAsync(ModuleBaseHelper.InvalidConfig(time, ModuleBaseHelper.ValidHoursString));
                    return;
                case JobConfigState.InvalidMinutes:
                case JobConfigState.InvalidSeconds:
                    await ReplyAsync(ModuleBaseHelper.InvalidConfig(time, ModuleBaseHelper.ValidMinutesSecondsString));
                    return;
                case JobConfigState.InvalidTime:
                    await ReplyAsync($"{time} is an invalid {nameof(Time)}.");
                    return;
                case JobConfigState.IntervalTooSmall:
                case JobConfigState.IntervallTooBig:
                    await ReplyAsync(ModuleBaseHelper.InvalidConfigRange(_GlobalConfig.Config.MinimumSeconds, _GlobalConfig.Config.MaximumSeconds));
                    return;
                case JobConfigState.Valid:
                    await SaveConfig(interval, time);
                    await Get();
                    return;
            }
        }
        [Command(nameof(Remove))]
        [Summary("Removes the " + nameof(JobConfig) + " for this channel.")]
        [Example("!" + nameof(JobConfig) + " " + nameof(Remove))]
        public async Task Remove()
        {
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                await SendRemoveMessage();
                await _GlobalConfig.JobConfigDal.Remove(Context.Channel.Id);
                var postImageJob = _GlobalConfig.Jobs.OfType<PostImageJob>().Single(s => s.ChannelIds.Contains(Context.Channel.Id));
                postImageJob.ChannelIds.Remove(Context.Channel.Id);
                await RemoveTimerIfNoChannels(postImageJob);
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        static string Alternate(string s) => string.Concat(s.ToLower().AsEnumerable().Select((c, i) => i % 2 == 0 ? c : char.ToUpper(c)));
        Task RemoveTimerIfNoChannels(PostImageJob postImageJob)
        {
            if (!postImageJob.ChannelIds.Any())
            {
                _GlobalConfig.Jobs.Remove(postImageJob);
                postImageJob?.Dispose();
            }
            return Task.CompletedTask;
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
            await UpdateJobConfigTable(config);
            await UpdateJob(interval, time, config);
        }
        async Task UpdateJobConfigTable(JobConfig config)
        {
            var any = await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id);
            if (any)
                await _GlobalConfig.JobConfigDal.Update(config);
            else
                await _GlobalConfig.JobConfigDal.Insert(config);
        }
        async Task UpdateJob(int interval, Time time, JobConfig config)
        {
            var postImageJobs = _GlobalConfig.Jobs.OfType<PostImageJob>().ToList();
            var existingJob = postImageJobs.SingleOrDefault(s => s.ChannelIds.Contains(Context.Channel.Id));
            if (existingJob != null)
            {
                existingJob.ChannelIds.Remove(Context.Channel.Id);
                await RemoveTimerIfNoChannels(existingJob);
            }
            var postImageJob = postImageJobs.SingleOrDefault(s => s.Interval == interval && s.Time == time);
            if (postImageJob == null)
                await AddJobConfig(config);
            else
                postImageJob.ChannelIds.Add(Context.Channel.Id);
        }
        Task AddJobConfig(JobConfig config)
        {
            var postImageJob = new PostImageJob(_Services, config);
            postImageJob.ChannelIds.Add(Context.Channel.Id);
            _GlobalConfig.Jobs.Add(postImageJob);
            postImageJob.StartTimerWithCloseInterval();
            return Task.CompletedTask;
        }
    }
}
