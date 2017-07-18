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
            await ReplyAsync($"Visit {_GlobalConfig.Config.GitHubUrl} for help!");
            //await SendHelpMenu();
        }
        async Task SendHelpMenu()
        {
            var avatarUrl = Context.Client.CurrentUser.GetAvatarUrl();
            var author = new EmbedAuthorBuilder()
                .WithName(nameof(JobConfig))
                .WithIconUrl(avatarUrl);
            var fields = ModuleBaseHelper.BuildFields<JobConfigModule>();
            var embed = new EmbedBuilder { Fields = fields }
                .WithAuthor(author)
                .WithDescription($"Commands for interacting with {nameof(JobConfig)}.\n- You can configure this bot in different channels on your server. Configuration is saved per channel.\n- You have complete control over how often it posts. Have fun with it! If it gets annoying or something, just dial it back a bit.");
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
            var state = JobHelper.DetermineJobConfigState(interval, time, _GlobalConfig.Config);
            switch (state)
            {
                case JobConfigState.InvalidHours:
                    var validHours = string.Join(", ", _GlobalConfig.Config.ValidHours);
                    await base.ReplyAsync(ModuleBaseHelper.InvalidConfig(time, validHours));
                    return;
                case JobConfigState.InvalidMinutes:
                case JobConfigState.InvalidSeconds:
                    var validMinutes = string.Join(", ", _GlobalConfig.Config.ValidMinutes);
                    await base.ReplyAsync(ModuleBaseHelper.InvalidConfig(time, validMinutes));
                    return;
                case JobConfigState.InvalidTime:
                    await ReplyAsync($"{time} is an invalid {nameof(Time)}.");
                    return;
                case JobConfigState.IntervalTooSmall:
                case JobConfigState.IntervallTooBig:
                    await ReplyAsync(ModuleBaseHelper.InvalidConfigRange(_GlobalConfig.Config.MinJobConfig, _GlobalConfig.Config.MaxJobConfig));
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
                await _GlobalConfig.JobConfigDal.Remove(Context.Channel.Id);
                var postImageJob = _GlobalConfig.Jobs.OfType<PostImageJob>().Single(s => s.ChannelIds != null && s.ChannelIds.Contains(Context.Channel.Id));
                //TODO centralize remove logic?
                var match = postImageJob.JobConfigs.Single(s => s.ChannelId == Context.Channel.Id);
                postImageJob.JobConfigs.Remove(match);
                await RemoveJobIfNoChannels(postImageJob);
                await SendRemoveMessage();
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        static string Alternate(string s) => string.Concat(s.ToLower().AsEnumerable().Select((c, i) => i % 2 == 0 ? c : char.ToUpper(c)));
        Task RemoveJobIfNoChannels(PostImageJob postImageJob)
        {
            if (!postImageJob.JobConfigs.Any())
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
            //TODO centralize remove logic?
            var postImageJobs = _GlobalConfig.Jobs.OfType<PostImageJob>().ToList();
            var existingJob = postImageJobs.SingleOrDefault(s => s.ChannelIds != null && s.ChannelIds.Contains(Context.Channel.Id));
            if (existingJob != null)
            {
                var match = existingJob.JobConfigs.Single(s => s.ChannelId == Context.Channel.Id);
                existingJob.JobConfigs.Remove(match);
                await RemoveJobIfNoChannels(existingJob);
            }
            var postImageJob = postImageJobs.SingleOrDefault(s => s.Interval == interval && s.Time == time);
            if (postImageJob == null)
                await AddJobConfig(config);
            else
                postImageJob.JobConfigs.Add(config);
        }
        Task AddJobConfig(JobConfig config)
        {
            var postImageJob = new PostImageJob(_Services, config);
            postImageJob.JobConfigs.Add(config);
            _GlobalConfig.Jobs.Add(postImageJob);
            postImageJob.StartTimerWithCloseInterval();
            return Task.CompletedTask;
        }
    }
}
