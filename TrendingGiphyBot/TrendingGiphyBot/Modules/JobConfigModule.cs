using System.Threading.Tasks;
using Discord.Commands;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Dals;
using Discord;
using System;
using System.Collections.Generic;
using TrendingGiphyBot.Configuration;
using NLog;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(JobConfig))]
    public class JobConfigModule : LoggingModuleBase
    {
        public JobConfigModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()){}
        string NotConfiguredMessage => $"{Context.Channel.Id} not configured. Configure me senpai! Use '{GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)}' or '{GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)} {nameof(Help)}' to learn how to.";
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help()
        {
            await ReplyAsync(string.Empty, embed: GlobalConfig.HelpMessagEmbed.Value);
        }
        [Command(nameof(Get))]
        public async Task Get()
        {
            var any = await GlobalConfig.JobConfigDal.Any(Context.Channel.Id);
            if (any)
            {
                var config = await GlobalConfig.JobConfigDal.Get(Context.Channel.Id);
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
            var state = GlobalConfig.Config.DetermineJobConfigState(interval, time);
            switch (state)
            {
                case JobConfigState.InvalidHours:
                    await ReplyAsync(InvalidConfigMessage(time, GlobalConfig.Config.ValidHours));
                    return;
                case JobConfigState.InvalidMinutes:
                    await ReplyAsync(InvalidConfigMessage(time, GlobalConfig.Config.ValidMinutes));
                    return;
                case JobConfigState.InvalidSeconds:
                    await ReplyAsync(InvalidConfigMessage(time, GlobalConfig.Config.ValidSeconds));
                    return;
                case JobConfigState.InvalidTime:
                    await ReplyAsync($"{time} is an invalid {nameof(Time)}.");
                    return;
                case JobConfigState.IntervalTooSmall:
                case JobConfigState.IntervallTooBig:
                    await ReplyAsync(InvalidConfigRangeMessage(GlobalConfig.Config.MinJobConfig, GlobalConfig.Config.MaxJobConfig));
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
            if (await GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                await GlobalConfig.JobConfigDal.Remove(Context.Channel.Id);
                await ReplyAsync("Configuration removed.");
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        async Task SaveConfig(int interval, Time time)
        {
            var config = new JobConfig
            {
                ChannelId = Context.Channel.Id,
                Interval = interval,
                Time = time.ToString()
            };
            await UpdateJobConfigTable(config);
        }
        async Task UpdateJobConfigTable(JobConfig config)
        {
            if (await GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
                await GlobalConfig.JobConfigDal.Update(config);
            else
                await GlobalConfig.JobConfigDal.Insert(config);
        }
        static string InvalidConfigMessage(Time time, List<int> validValues) =>
            $"When {nameof(Time)} is {time}, interval must be {validValues.FlattenWith(", ")}.";
        static string InvalidConfigRangeMessage(SubJobConfig minConfig, SubJobConfig maxConfig) =>
            $"Interval must be between {minConfig.Interval} {minConfig.Time} and {maxConfig.Interval} {maxConfig.Time}.";
    }
}
