using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Extensions;

namespace TrendingGiphyBot.Modules
{
    [Group("Trend")]
    public class TrendModule : ModuleBase
    {
        readonly ILogger _Logger;
        readonly IGlobalConfig _GlobalConfig;
        static readonly char[] _ArgsSplit = new[] { ' ' };
        public TrendModule(IServiceProvider services)
        {
            _Logger = LogManager.GetCurrentClassLogger();
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        [Command(nameof(Get))]
        [Alias(nameof(Get), "", "Config", "Setup", "Help")]
        public async Task Get()
        {
            using (var entities = _GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
            {
                var isConfigured = await entities.AnyJobConfig(Context.Channel.Id);
                if (isConfigured)
                    await Get(entities);
                else
                    await NotConfiguredReplyAsync();
            }
        }
        [Command(nameof(Off))]
        [Alias("Remove", "Stop")]
        public async Task Off()
        {
            using (var entities = _GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                await entities.RemoveJobConfig(Context.Channel.Id);
            await TryReplyAsync("Done.");
        }
        [Command(nameof(Every))]
        public async Task Every(int interval, Time time)
        {
            var state = _GlobalConfig.Config.DetermineJobConfigState(interval, time);
            switch (state)
            {
                case JobConfigState.InvalidHours:
                    await TryReplyAsync(InvalidConfigMessage(time, _GlobalConfig.Config.ValidHours));
                    return;
                case JobConfigState.InvalidMinutes:
                    await TryReplyAsync(InvalidConfigMessage(time, _GlobalConfig.Config.ValidMinutes));
                    return;
                case JobConfigState.InvalidSeconds:
                    await TryReplyAsync(InvalidConfigMessage(time, _GlobalConfig.Config.ValidSeconds));
                    return;
                case JobConfigState.InvalidTime:
                    await TryReplyAsync($"{time} is an invalid {nameof(Time)}.");
                    return;
                case JobConfigState.IntervalTooSmall:
                case JobConfigState.IntervallTooBig:
                    await TryReplyAsync(InvalidConfigRangeMessage(_GlobalConfig.Config.MinJobConfig, _GlobalConfig.Config.MaxJobConfig));
                    return;
                case JobConfigState.Valid:
                    using (var entities = _GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
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
            using (var entities = _GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
            {
                var isConfigured = await entities.AnyJobConfig(Context.Channel.Id);
                if (isConfigured)
                    if (randomSearchString.Equals("off", StringComparison.CurrentCultureIgnoreCase))
                    {
                        await entities.TurnOffRandom(Context.Channel.Id);
                        await Get(entities);
                    }
                    else
                    {
                        var cleanedRandomSearchString = CleanRandomSearchString(randomSearchString);
                        var isValidRandomSearchString = string.IsNullOrWhiteSpace(cleanedRandomSearchString) || cleanedRandomSearchString.Length <= _GlobalConfig.Config.RandomSearchStringMaxLength;
                        if (isValidRandomSearchString)
                        {
                            await entities.UpdateRandom(Context.Channel.Id, true, cleanedRandomSearchString);
                            await Get(entities);
                        }
                        else
                            await TryReplyAsync($"Random search string must be at most {_GlobalConfig.Config.RandomSearchStringMaxLength} characters long.");
                    }
                else
                    await NotConfiguredReplyAsync();
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
            using (var entities = _GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
            {
                var isConfigured = await entities.AnyJobConfig(Context.Channel.Id);
                if (isConfigured)
                {
                    if (!string.IsNullOrWhiteSpace(quietHoursString))
                        if (quietHoursString.Equals("off", StringComparison.CurrentCultureIgnoreCase))
                        {
                            await entities.TurnOffQuietHours(Context.Channel.Id);
                            await Get(entities);
                        }
                        else
                            await UpdateQuietHours(quietHoursString);
                }
                else
                    await NotConfiguredReplyAsync();
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
                        using (var entities = _GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                        {
                            await entities.UpdateQuietHoursWithHourOffset(Context.Channel.Id, minHour, maxHour, _GlobalConfig.Config.HourOffset);
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
        static string InvalidConfigMessage(Time time, List<int> validValues) =>
            $"When {nameof(Time)} is {time}, interval must be {string.Join(", ", validValues)}.";
        static string InvalidConfigRangeMessage(SubJobConfig minConfig, SubJobConfig maxConfig) =>
            $"Interval must be between {minConfig.Interval} {minConfig.Time} and {maxConfig.Interval} {maxConfig.Time}.";
        async Task Get(TrendingGiphyBotEntities entities)
        {
            var config = await entities.GetJobConfigWithHourOffset(Context.Channel.Id, _GlobalConfig.Config.HourOffset);
            var avatarUrl = (await Context.Client.GetGuildAsync(Context.Guild.Id)).IconUrl;
            var author = new EmbedAuthorBuilder()
                .WithName("Your Trending Giphy Bot Setup")
                .WithIconUrl(avatarUrl);
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(author)
                .WithDescription(_GlobalConfig.Config.HelpMessage.Description)
                .AddInlineField(nameof(config.Interval), config.Interval)
                .AddInlineField(nameof(config.Time), config.Time)
                .WithRandomConfigFields(config)
                .WithQuietHourFields(config, _GlobalConfig.Config.HourOffset);
            await TryReplyAsync(embedBuilder);
        }
        async Task NotConfiguredReplyAsync()
        {
            var notConfiguredEmbed = _GlobalConfig.BuildEmbedFromConfig(_GlobalConfig.Config.NotConfiguredMessage);
            await TryReplyAsync(notConfiguredEmbed);
        }
        async Task HelpMessageReplyAsync()
        {
            var helpMessageEmbed = _GlobalConfig.BuildEmbedFromConfig(_GlobalConfig.Config.HelpMessage);
            await TryReplyAsync(helpMessageEmbed);
        }
        async Task TryReplyAsync(string message) => await TryReplyAsync(message, null);
        async Task TryReplyAsync(EmbedBuilder embedBuilder) => await TryReplyAsync(string.Empty, embedBuilder);
        async Task TryReplyAsync(string message, EmbedBuilder embedBuilder)
        {
            try
            {
                await ReplyAsync(message, embed: embedBuilder);
            }
            catch (HttpException httpException) when (_GlobalConfig.Config.HttpExceptionsToWarn.Contains(httpException.Message))
            {
                _Logger.Warn(httpException.Message);
                await _GlobalConfig.MessageHelper.SendMessageToUser(Context, message, embedBuilder);
            }
        }
        protected override void BeforeExecute(CommandInfo command) => _Logger.Trace($"Calling {command.Name}.");
        protected override void AfterExecute(CommandInfo command) => _Logger.Trace($"{command.Name} done.");
    }
}
