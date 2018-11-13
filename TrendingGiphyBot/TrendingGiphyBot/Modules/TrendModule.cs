using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using NLog;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Extensions;

namespace TrendingGiphyBot.Modules
{
    [Group("Trend")]
    public class TrendModule : BotModuleBase
    {
        static readonly char[] _ArgsSplit = new[] { ' ' };
        public TrendModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()) { }
        [Command(nameof(Get))]
        [Alias(nameof(Get), "", "Config", "Setup", "Help")]
        public async Task Get()
        {
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
            {
                var isConfigured = await entities.AnyJobConfig(Context.Channel.Id);
                if (isConfigured)
                    await Get(entities);
                else
                    await HelpMessageReplyAsync();
            }
        }
        [Command(nameof(Off))]
        [Alias("Remove", "Stop")]
        public async Task Off()
        {
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                await entities.RemoveJobConfig(Context.Channel.Id);
            await TryReplyAsync("Done.");
        }
        [Command(nameof(Every))]
        public async Task Every(int interval, Time time)
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
                    using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                    {
                        await entities.UpdateInterval(Context.Channel.Id, interval, time);
                        await Get(entities);
                    }
                    return;
            }
        }
        [Command("Random")]
        public async Task TrendRandom([Remainder] string randomSearchString = null)
        {
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
            {
                var isConfigured = await entities.AnyJobConfig(Context.Channel.Id);
                if (isConfigured)
                {
                    if (randomSearchString.Equals("off", StringComparison.CurrentCultureIgnoreCase))
                        await TurnOffRandom(entities);
                    else
                    {
                        var cleanedRandomSearchString = CleanRandomSearchString(randomSearchString);
                        var isValidRandomSearchString = string.IsNullOrWhiteSpace(cleanedRandomSearchString) || cleanedRandomSearchString.Length <= GlobalConfig.Config.RandomSearchStringMaxLength;
                        if (isValidRandomSearchString)
                        {
                            await entities.UpdateRandom(Context.Channel.Id, true, cleanedRandomSearchString);
                            await Get(entities);
                        }
                        else
                            await TryReplyAsync($"Random search string must be at most {GlobalConfig.Config.RandomSearchStringMaxLength} characters long.");
                    }
                }
                else
                    await HelpMessageReplyAsync();
            }
        }
        static string CleanRandomSearchString(string randomSearchString)
        {
            if (!string.IsNullOrEmpty(randomSearchString))
            {
                if (randomSearchString.StartsWith("gifs of", StringComparison.CurrentCultureIgnoreCase))
                    return randomSearchString.Substring(7).TrimStart();
                if (randomSearchString.StartsWith("gif of", StringComparison.CurrentCultureIgnoreCase))
                    return randomSearchString.Substring(6).TrimStart();
                if (randomSearchString.StartsWith("gifs", StringComparison.CurrentCultureIgnoreCase))
                    return randomSearchString.Substring(4).TrimStart();
                if (randomSearchString.StartsWith("gif", StringComparison.CurrentCultureIgnoreCase))
                    return randomSearchString.Substring(3).TrimStart();
                return randomSearchString;
            }
            return null;
        }
        [Command(nameof(QuietHours))]
        public async Task QuietHours([Remainder] string quietHoursString = null)
        {
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
            {
                var isConfigured = await entities.AnyJobConfig(Context.Channel.Id);
                if (isConfigured)
                {
                    if (!string.IsNullOrWhiteSpace(quietHoursString))
                        if (quietHoursString.Equals("off", StringComparison.CurrentCultureIgnoreCase))
                            await TurnOffQuietHours();
                        else
                            await UpdateQuietHours(quietHoursString);
                }
                else
                    await HelpMessageReplyAsync();
            }
        }
        async Task UpdateQuietHours(string quietHoursString)
        {
            var split = quietHoursString.Split(_ArgsSplit, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 2)
            {
                var minHourSuccess = short.TryParse(split[0], out var minHour);
                if (minHourSuccess)
                {
                    var maxHourSuccess = short.TryParse(split[1], out var maxHour);
                    if (maxHourSuccess)
                        using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                        {
                            var isConfigured = await entities.AnyJobConfig(Context.Channel.Id);
                            if (isConfigured)
                            {
                                await entities.UpdateQuietHoursWithHourOffset(Context.Channel.Id, minHour, maxHour, GlobalConfig.Config.HourOffset);
                                await Get(entities);
                            }
                            else
                                await HelpMessageReplyAsync();
                        }
                    else
                        await HelpMessageReplyAsync();
                }
                else
                    await HelpMessageReplyAsync();
            }
            else
                await HelpMessageReplyAsync();
        }
        async Task TurnOffRandom(TrendingGiphyBotEntities entities)
        {
            var isConfigured = await entities.AnyJobConfig(Context.Channel.Id);
            if (isConfigured)
            {
                await entities.TurnOffRandom(Context.Channel.Id);
                await Get(entities);
            }
            else
                await HelpMessageReplyAsync();
        }
        async Task TurnOffQuietHours()
        {
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
            {
                var isConfigured = await entities.AnyJobConfig(Context.Channel.Id);
                if (isConfigured)
                {
                    await entities.TurnOffQuietHours(Context.Channel.Id);
                    await Get(entities);
                }
                else
                    await HelpMessageReplyAsync();
            }
        }
        static string InvalidConfigMessage(Time time, List<int> validValues) =>
            $"When {nameof(Time)} is {time}, interval must be {string.Join(", ", validValues)}.";
        static string InvalidConfigRangeMessage(SubJobConfig minConfig, SubJobConfig maxConfig) =>
            $"Interval must be between {minConfig.Interval} {minConfig.Time} and {maxConfig.Interval} {maxConfig.Time}.";
        async Task Get(TrendingGiphyBotEntities entities)
        {
            var config = await entities.GetJobConfigWithHourOffset(Context.Channel.Id, GlobalConfig.Config.HourOffset);
            var avatarUrl = (await Context.Client.GetGuildAsync(Context.Guild.Id)).IconUrl;
            var author = new EmbedAuthorBuilder()
                .WithName("Your Trending Giphy Bot Setup")
                .WithIconUrl(avatarUrl);
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(author)
                .WithDescription(GlobalConfig.Config.HelpMessage.Description)
                .AddInlineField(nameof(config.Interval), config.Interval)
                .AddInlineField(nameof(config.Time), config.Time)
                .WithRandomConfigFields(config)
                .WithQuietHourFields(config, GlobalConfig.Config.HourOffset);
            await TryReplyAsync(embedBuilder);
        }
    }
}
