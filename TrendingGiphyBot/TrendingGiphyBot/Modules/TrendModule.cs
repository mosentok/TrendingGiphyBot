﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Containers;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Extensions;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Modules
{
    [Group("Trend")]
    public class TrendModule : ModuleBase, IDisposable
    {
        readonly ILogger _Logger;
        readonly IGlobalConfig _GlobalConfig;
        readonly TrendingGiphyBotEntities _Entities;
        readonly ITrendHelper _TrendHelper;
        readonly IFunctionHelper _FunctionHelper;
        static readonly char[] _ArgsSplit = new[] { ' ' };
        public TrendModule(IServiceProvider services)
        {
            _Logger = LogManager.GetCurrentClassLogger();
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
            _Entities = _GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities();
            _TrendHelper = services.GetRequiredService<ITrendHelper>();
            _FunctionHelper = services.GetRequiredService<IFunctionHelper>();
        }
        [Command(nameof(Get))]
        [Alias(nameof(Get), "", "Config", "Setup")]
        public async Task Get()
        {
            var jobConfig = await _FunctionHelper.GetJobConfigAsync(Context.Channel.Id);
            await ReplyWithJobConfig(jobConfig);
        }
        [Command(nameof(Off))]
        public async Task Off()
        {
            await _Entities.RemoveJobConfig(Context.Channel.Id);
            await TryReplyAsync("Done.");
        }
        [Command(nameof(Every))]
        public async Task Every(int interval, Time time)
        {
            var state = _GlobalConfig.Config.DetermineJobConfigState(interval, time);
            switch (state)
            {
                case JobConfigState.InvalidHours:
                    var invalidHoursMessage = _TrendHelper.InvalidConfigMessage(time, _GlobalConfig.Config.ValidHours);
                    await TryReplyAsync(invalidHoursMessage);
                    return;
                case JobConfigState.InvalidMinutes:
                    var invalidMinutesMessage = _TrendHelper.InvalidConfigMessage(time, _GlobalConfig.Config.ValidMinutes);
                    await TryReplyAsync(invalidMinutesMessage);
                    return;
                case JobConfigState.InvalidSeconds:
                    var invalidConfigMessage = _TrendHelper.InvalidConfigMessage(time, _GlobalConfig.Config.ValidSeconds);
                    await TryReplyAsync(invalidConfigMessage);
                    return;
                case JobConfigState.InvalidTime:
                    await TryReplyAsync($"{time} is an invalid {nameof(Time)}.");
                    return;
                case JobConfigState.IntervalTooSmall:
                case JobConfigState.IntervallTooBig:
                    var invalidConfigRangeMessage = _TrendHelper.InvalidConfigRangeMessage(_GlobalConfig.Config.MinJobConfig, _GlobalConfig.Config.MaxJobConfig);
                    await TryReplyAsync(invalidConfigRangeMessage);
                    return;
                case JobConfigState.Valid:
                    var match = await _FunctionHelper.GetJobConfigAsync(Context.Channel.Id);
                    await SetJobConfig(match, interval, time);
                    return;
            }
        }
        async Task SetJobConfig(JobConfigContainer match, int interval, Time time)
        {
            JobConfigContainer container;
            if (match != null)
                container = new JobConfigContainer(match, interval, time.ToString());
            else
                container = new JobConfigContainer(Context.Channel.Id, interval, time.ToString());
            var result = await _FunctionHelper.SetJobConfigAsync(Context.Channel.Id, container);
            await ReplyWithJobConfig(result);
        }
        [Command("Random")]
        public async Task TrendRandom([Remainder] string randomSearchString = null)
        {
            var match = await _FunctionHelper.GetJobConfigAsync(Context.Channel.Id);
            await ProcessRandomRequest(randomSearchString, match);
        }
        Task ProcessRandomRequest(string randomSearchString, JobConfigContainer match)
        {
            if (match == null)
                return ExamplesReplyAsync(true);
            var cleanedRandomSearchString = _TrendHelper.CleanRandomSearchString(randomSearchString);
            var isValidRandomSearchString = _TrendHelper.IsValidRandomSearchString(cleanedRandomSearchString, _GlobalConfig.Config.RandomSearchStringMaxLength);
            if (!isValidRandomSearchString)
                return TryReplyAsync($"Random search string must be at most {_GlobalConfig.Config.RandomSearchStringMaxLength} characters long.");
            var shouldTurnCommandOff = _TrendHelper.ShouldTurnCommandOff(randomSearchString);
            if (shouldTurnCommandOff)
                return SetRandom(match, false, null);
            return SetRandom(match, true, cleanedRandomSearchString);
        }
        async Task SetRandom(JobConfigContainer match, bool randomIsOn, string randomSearchString)
        {
            var container = new JobConfigContainer(match, randomIsOn, randomSearchString);
            var result = await _FunctionHelper.SetJobConfigAsync(Context.Channel.Id, container);
            await ReplyWithJobConfig(result);
        }
        [Command(nameof(Between))]
        public async Task Between([Remainder] string trendBetweenString)
        {
            var match = await _FunctionHelper.GetJobConfigAsync(Context.Channel.Id);
            await ProcessBetweenRequest(trendBetweenString, match);
        }
        Task ProcessBetweenRequest(string trendBetweenString, JobConfigContainer match)
        {
            if (match == null)
                return ExamplesReplyAsync(true);
            var shouldTurnCommandOff = _TrendHelper.ShouldTurnCommandOff(trendBetweenString);
            if (shouldTurnCommandOff)
                return SetBetween(match, null, null);
            var split = trendBetweenString.Split(_ArgsSplit, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 3)
                return TryReplyAsync(_GlobalConfig.Config.InvalidQuietHoursInputFormat);
            var minInRange = _TrendHelper.IsInRange(split[2], out var minQuietHour);
            var maxInRange = _TrendHelper.IsInRange(split[0], out var maxQuietHour);
            if (!(minInRange && maxInRange) || minQuietHour == maxQuietHour)
                return TryReplyAsync(_GlobalConfig.Config.InvalidQuietHoursRangeMessage);
            return SetBetween(match, minQuietHour, maxQuietHour);
        }
        async Task SetBetween(JobConfigContainer match, short? minQuietHour, short? maxQuietHour)
        {
            var container = new JobConfigContainer(match, minQuietHour, maxQuietHour);
            var result = await _FunctionHelper.SetJobConfigAsync(Context.Channel.Id, container);
            await ReplyWithJobConfig(result);
        }
        [Command(nameof(Prefix))]
        public async Task Prefix(string prefix)
        {
            var isConfigured = await _Entities.AnyJobConfig(Context.Channel.Id);
            if (isConfigured)
                if (_TrendHelper.ShouldTurnCommandOff(prefix))
                {
                    await _Entities.SetPrefix(Context.Channel.Id, _GlobalConfig.Config.DefaultPrefix);
                    await GetJobConfig();
                }
                else
                {
                    var isValid = !string.IsNullOrEmpty(prefix) && prefix.Length <= 4;
                    if (isValid)
                    {
                        if (await _Entities.AnyChannelConfigs(Context.Channel.Id))
                            await _Entities.SetPrefix(Context.Channel.Id, prefix);
                        else
                            await _Entities.InsertChannelConfig(Context.Channel.Id, prefix);
                        await GetJobConfig();
                    }
                    else
                        await TryReplyAsync("Prefix must be 1-4 characters long.");
                }
            else
                await ExamplesReplyAsync(true);
        }
        [Command(nameof(Examples))]
        [Alias("Example", "Help")]
        public async Task Examples() => await ExamplesReplyAsync(false);
        async Task ExamplesReplyAsync(bool includeNotConfiguredMessage)
        {
            var description = DetermineExamplesDescription(includeNotConfiguredMessage);
            var author = new EmbedAuthorBuilder()
                .WithName("Trending Giphy Bot Examples")
                .WithIconUrl(Context.Guild.IconUrl);
            var helpField = new EmbedFieldBuilder()
                .WithName("Need More Help?")
                .WithValue(_GlobalConfig.Config.ExamplesHelpFieldText);
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(author)
                .WithDescription(description)
                .AddField(helpField);
            await TryReplyAsync(embedBuilder);
        }
        string DetermineExamplesDescription(bool includeNotConfiguredMessage)
        {
            if (includeNotConfiguredMessage)
                return _GlobalConfig.Config.NotConfiguredMessageStart + _GlobalConfig.Config.ExamplesText;
            return _GlobalConfig.Config.ExamplesText;
        }
        async Task GetJobConfig()
        {
            var config = await _FunctionHelper.GetJobConfigAsync(Context.Channel.Id);
            await ReplyWithJobConfig(config);
        }
        async Task ReplyWithJobConfig(JobConfigContainer config)
        {
            if (config != null)
            {
                var author = new EmbedAuthorBuilder()
                    .WithName($"Setup for Channel # {Context.Channel.Name}")
                    .WithIconUrl(Context.Guild.IconUrl);
                var helpField = new EmbedFieldBuilder()
                    .WithName("Need Help?")
                    .WithValue(_GlobalConfig.Config.GetConfigHelpFieldText);
                var embedBuilder = new EmbedBuilder()
                    .WithAuthor(author)
                    .WithHowOften(config)
                    .WithRandomConfigFields(config)
                    .WithQuietHourFields(config)
                    .AddField(helpField);
                await TryReplyAsync(embedBuilder);
            }
            else
                await ExamplesReplyAsync(true);
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
        public void Dispose()
        {
            _Entities?.Dispose();
        }
    }
}
