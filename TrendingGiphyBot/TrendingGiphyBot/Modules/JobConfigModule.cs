using System.Threading.Tasks;
using Discord.Commands;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Helpers;
using TrendingGiphyBot.Jobs;
using System.Linq;
using Discord;
using System;
using TrendingGiphyBot.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(JobConfig))]
    public class JobConfigModule : ModuleBase
    {
        readonly IServiceProvider _Services;
        readonly IGlobalConfig _GlobalConfig;
        public JobConfigModule(IServiceProvider services)
        {
            _Services = services;
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        string NotConfiguredMessage => $"{Context.Channel.Id} not configured. Configure me senpai! Use '{_GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)}' or '{_GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)} {nameof(Help)}' to learn how to.";
        [Command(nameof(Help))]
        public async Task Help()
        {
            await ReplyAsync($"Visit {_GlobalConfig.Config.GitHubUrl} for help!");
        }
        [Command(nameof(Get))]
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
        public async Task Set(int interval, Time time)
        {
            var state = _GlobalConfig.Config.DetermineJobConfigState(interval, time);
            switch (state)
            {
                case JobConfigState.InvalidHours:
                    var validHours = string.Join(", ", _GlobalConfig.Config.ValidHours);
                    await ReplyAsync(InvalidConfigMessage(time, validHours));
                    return;
                case JobConfigState.InvalidMinutes:
                    var validMinutes = string.Join(", ", _GlobalConfig.Config.ValidMinutes);
                    await ReplyAsync(InvalidConfigMessage(time, validMinutes));
                    return;
                case JobConfigState.InvalidSeconds:
                    var validSeconds = string.Join(", ", _GlobalConfig.Config.ValidSeconds);
                    await ReplyAsync(InvalidConfigMessage(time, validSeconds));
                    return;
                case JobConfigState.InvalidTime:
                    await ReplyAsync($"{time} is an invalid {nameof(Time)}.");
                    return;
                case JobConfigState.IntervalTooSmall:
                case JobConfigState.IntervallTooBig:
                    await ReplyAsync(InvalidConfigRangeMessage(_GlobalConfig.Config.MinJobConfig, _GlobalConfig.Config.MaxJobConfig));
                    return;
                case JobConfigState.Valid:
                    await SaveConfig(interval, time);
                    await Get();
                    return;
            }
        }
        [Command(nameof(Remove))]
        public async Task Remove()
        {
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                await _GlobalConfig.JobConfigDal.Remove(Context.Channel.Id);
                var postImageJob = _GlobalConfig.Jobs.OfType<PostImageJob>().Single(s => s.ChannelIds != null && s.ChannelIds.Contains(Context.Channel.Id));
                RemoveJobConfig(postImageJob);
                await RemoveJobIfNoChannels(postImageJob);
                await SendRemoveMessage();
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }

        Task RemoveJobIfNoChannels(PostImageJob postImageJob)
        {
            if (!postImageJob.JobConfigs.Any())
            {
                _GlobalConfig.Jobs.Remove(postImageJob);
                postImageJob.Dispose();
            }
            return Task.CompletedTask;
        }
        async Task SendRemoveMessage()
        {
             await ReplyAsync("Configuration removed.");
        }
        async Task SaveConfig(int interval, Time time)
        {
            var config = new JobConfig
            {
                ChannelId = Context.Channel.Id,
                Interval = interval,
                Time = time.ToString()
            };
            var fullConfig = await UpdateJobConfigTable(config);
            await UpdateJobWithNewConfig(interval, time, fullConfig);
        }
        async Task<JobConfig> UpdateJobConfigTable(JobConfig config)
        {
            var any = await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id);
            if (any)
                await _GlobalConfig.JobConfigDal.Update(config);
            else
                await _GlobalConfig.JobConfigDal.Insert(config);
            return await _GlobalConfig.JobConfigDal.Get(Context.Channel.Id);
        }
        async Task UpdateJobWithNewConfig(int interval, Time time, JobConfig fullJobConfig)
        {
            var postImageJobs = _GlobalConfig.Jobs.OfType<PostImageJob>().ToList();
            var existingJob = postImageJobs.SingleOrDefault(s => s.ChannelIds != null && s.ChannelIds.Contains(Context.Channel.Id));
            if (existingJob != null)
            {
                RemoveJobConfig(existingJob);
                await RemoveJobIfNoChannels(existingJob);
            }
            var postImageJob = postImageJobs.SingleOrDefault(s => s.Interval == interval && s.Time == time);
            if (postImageJob == null)
                await AddNewJobConfig(fullJobConfig);
            else
                postImageJob.JobConfigs.Add(fullJobConfig);
        }
        void RemoveJobConfig(PostImageJob postImageJob)
        {
            var match = postImageJob.JobConfigs.Single(s => s.ChannelId == Context.Channel.Id);
            postImageJob.JobConfigs.Remove(match);
        }
        Task AddNewJobConfig(JobConfig fullJobConfig)
        {
            var postImageJob = new PostImageJob(_Services, fullJobConfig.Interval, fullJobConfig.Time);
            postImageJob.JobConfigs.Add(fullJobConfig);
            _GlobalConfig.Jobs.Add(postImageJob);
            postImageJob.StartTimerWithCloseInterval();
            return Task.CompletedTask;
        }
        static string InvalidConfigMessage(Time time, string validValues) =>
            $"When {nameof(Time)} is {time}, interval must be {validValues}.";
        static string InvalidConfigRangeMessage(SubJobConfig minConfig, SubJobConfig maxConfig) =>
            $"Interval must be between {minConfig.Interval} {minConfig.Time} and {maxConfig.Interval} {maxConfig.Time}.";
    }
}
