using System.Threading.Tasks;
using Discord.Commands;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Dals;
using Discord;
using System;
using System.Collections.Generic;
using TrendingGiphyBot.Configuration;
using NLog;

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(JobConfig))]
    public class JobConfigModule : BotModuleBase
    {
        public JobConfigModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()) { }
        string NotConfiguredMessage => $"{Context.Channel.Id} not configured. Configure me senpai! Use '{GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)}' or '{GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)} {nameof(Help)}' to learn how to.";
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help() => await HelpMessageReplyAsync();
        [Command(nameof(Get))]
        public async Task Get()
        {
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                await Get(entities);
        }
        [Command(nameof(Set))]
        public async Task Set(int interval, Time time)
        {
            var state = GlobalConfig.Config.DetermineJobConfigState(interval, time);
            switch (state)
            {
                case JobConfigState.InvalidHours:
                    await TryReplyAsync(InvalidConfigMessage(time, GlobalConfig.Config.ValidHours));
                    return;
                case JobConfigState.InvalidMinutes:
                    await TryReplyAsync(InvalidConfigMessage(time, GlobalConfig.Config.ValidMinutes));
                    return;
                case JobConfigState.InvalidSeconds:
                    await TryReplyAsync(InvalidConfigMessage(time, GlobalConfig.Config.ValidSeconds));
                    return;
                case JobConfigState.InvalidTime:
                    await TryReplyAsync($"{time} is an invalid {nameof(Time)}.");
                    return;
                case JobConfigState.IntervalTooSmall:
                case JobConfigState.IntervallTooBig:
                    await TryReplyAsync(InvalidConfigRangeMessage(GlobalConfig.Config.MinJobConfig, GlobalConfig.Config.MaxJobConfig));
                    return;
                case JobConfigState.Valid:
                    await SaveConfig(interval, time);
                    using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                        await Get(entities);
                    return;
            }
        }
        [Command(nameof(Remove))]
        public async Task Remove()
        {
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                if (await entities.AnyJobConfig(Context.Channel.Id))
                {
                    await entities.RemoveJobConfig(Context.Channel.Id);
                    await TryReplyAsync("Configuration removed.");
                }
                else
                    await TryReplyAsync(NotConfiguredMessage);
        }
        async Task SaveConfig(int interval, Time time)
        {
            var config = new JobConfig
            {
                ChannelId = Context.Channel.Id,
                Interval = interval,
                Time = time.ToString()
            };
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                if (await entities.AnyJobConfig(Context.Channel.Id))
                    await entities.UpdateJobConfig(config);
                else
                    await entities.InsertJobConfig(config);
        }
        static string InvalidConfigMessage(Time time, List<int> validValues) =>
            $"When {nameof(Time)} is {time}, interval must be {string.Join(", ", validValues)}.";
        static string InvalidConfigRangeMessage(SubJobConfig minConfig, SubJobConfig maxConfig) =>
            $"Interval must be between {minConfig.Interval} {minConfig.Time} and {maxConfig.Interval} {maxConfig.Time}.";
        async Task Get(TrendingGiphyBotEntities entities)
        {
            var any = await entities.AnyJobConfig(Context.Channel.Id);
            if (any)
            {
                var config = await entities.GetJobConfig(Context.Channel.Id);
                var avatarUrl = (await Context.Client.GetGuildAsync(Context.Guild.Id)).IconUrl;
                var author = new EmbedAuthorBuilder()
                    .WithName($"{Context.Channel.Name}'s {nameof(JobConfig)}")
                    .WithIconUrl(avatarUrl);
                var embedBuilder = new EmbedBuilder()
                    .WithAuthor(author)
                    .AddInlineField(nameof(config.Interval), config.Interval)
                    .AddInlineField(nameof(config.Time), config.Time);
                await TryReplyAsync(embedBuilder);
            }
            else
                await TryReplyAsync(NotConfiguredMessage);
        }
    }
}
