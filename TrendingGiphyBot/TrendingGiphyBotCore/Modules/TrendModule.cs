﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotCore.Enums;
using TrendingGiphyBotCore.Helpers;
using TrendingGiphyBotCore.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotCore.Modules
{
    [Group("Trend")]
    public class TrendModule : ModuleBase
    {
        static readonly char[] _ArgsSplit = new[] { ' ' };
        readonly ILogger _Logger;
        readonly ITrendHelper _TrendHelper;
        readonly IFunctionWrapper _FunctionWrapper;
        readonly IConfigurationWrapper _Config;
        public TrendModule(IServiceProvider services)
        {
            _Logger = services.GetService<ILogger<TrendModule>>();
            _TrendHelper = services.GetRequiredService<ITrendHelper>();
            _FunctionWrapper = services.GetRequiredService<IFunctionWrapper>();
            _Config = services.GetService<IConfigurationWrapper>();
        }
        [Command(nameof(Get))]
        [Alias(nameof(Get), "", "Config", "Setup")]
        public async Task Get()
        {
            var jobConfig = await _FunctionWrapper.GetJobConfigAsync(Context.Channel.Id);
            await ReplyWithJobConfig(jobConfig);
        }
        [Command(nameof(Off))]
        public async Task Off()
        {
            await _FunctionWrapper.DeleteJobConfigAsync(Context.Channel.Id);
            await TryReplyAsync("Done.");
        }
        [Command(nameof(Every))]
        public async Task Every(short interval, Time time)
        {
            var errorMessage = _TrendHelper.DetermineErrorMessage(interval, time);
            if (string.IsNullOrEmpty(errorMessage))
            {
                var match = await _FunctionWrapper.GetJobConfigAsync(Context.Channel.Id);
                var container = new JobConfigContainer(match, interval, time.ToString());
                var result = await _FunctionWrapper.PostJobConfigAsync(Context.Channel.Id, container);
                await ReplyWithJobConfig(result);
            }
            else
                await TryReplyAsync(errorMessage);
        }
        [Command("Random")]
        public async Task TrendRandom([Remainder] string randomSearchString)
        {
            var match = await _FunctionWrapper.GetJobConfigAsync(Context.Channel.Id);
            var maxLength = _Config.GetValue<int>("RandomSearchStringMaxLength");
            var isValidSearchString = !string.IsNullOrEmpty(randomSearchString) && randomSearchString.Length <= maxLength;
            if (!isValidSearchString)
                await TryReplyAsync($"Please provide a random gif filter that is at most {maxLength} characters long.");
            else
            {
                string newRandomSearchString;
                var shouldTurnCommandOff = _TrendHelper.ShouldTurnCommandOff(randomSearchString);
                if (shouldTurnCommandOff)
                    newRandomSearchString = null;
                else
                    newRandomSearchString = randomSearchString;
                var container = new JobConfigContainer(match, randomSearchString);
                var result = await _FunctionWrapper.PostJobConfigAsync(Context.Channel.Id, container);
                await ReplyWithJobConfig(result);
            }
        }
        [Command(nameof(Between))]
        public async Task Between([Remainder] string trendBetweenString)
        {
            var match = await _FunctionWrapper.GetJobConfigAsync(Context.Channel.Id);
            var split = trendBetweenString.Split(_ArgsSplit, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 3)
                await TryReplyAsync(_Config["InvalidQuietHoursInputFormat"]);
            else
            {
                var minInRange = _TrendHelper.IsInRange(split[2], out var minQuietHour);
                var maxInRange = _TrendHelper.IsInRange(split[0], out var maxQuietHour);
                if (!(minInRange && maxInRange) || minQuietHour == maxQuietHour)
                    await TryReplyAsync(_Config["InvalidQuietHoursRangeMessage"]);
                else
                {
                    short? newMinQuietHour;
                    short? newMaxQuietHour;
                    var shouldTurnCommandOff = _TrendHelper.ShouldTurnCommandOff(trendBetweenString);
                    if (shouldTurnCommandOff)
                    {
                        newMinQuietHour = null;
                        newMaxQuietHour = null;
                    }
                    else
                    {
                        newMinQuietHour = minQuietHour;
                        newMaxQuietHour = maxQuietHour;
                    }
                    var container = new JobConfigContainer(match, minQuietHour, maxQuietHour);
                    var result = await _FunctionWrapper.PostJobConfigAsync(Context.Channel.Id, container);
                    await ReplyWithJobConfig(result);
                }
            }
        }
        [Command(nameof(Prefix))]
        public async Task Prefix(string prefix)
        {
            var match = await _FunctionWrapper.GetJobConfigAsync(Context.Channel.Id);
            if (string.IsNullOrEmpty(prefix) || prefix.Length > 4)
                await TryReplyAsync("Prefix must be 1-4 characters long.");
            else
            {
                var shouldTurnCommandOff = _TrendHelper.ShouldTurnCommandOff(prefix);
                if (shouldTurnCommandOff)
                    match.Prefix = _Config["DefaultPrefix"];
                else
                    match.Prefix = prefix;
                var result = await _FunctionWrapper.PostJobConfigAsync(Context.Channel.Id, match);
                await TryReplyAsync($"New prefix: {result.Prefix}");
            }
            _TrendHelper.OnPrefixUpdated(match.ChannelId, prefix);
        }
        [Command(nameof(Examples))]
        [Alias("Example", "Help")]
        public async Task Examples()
        {
            var author = new EmbedAuthorBuilder()
                .WithName("Trending Giphy Bot Examples")
                .WithIconUrl(Context.Guild.IconUrl);
            var helpFieldText = _Config["ExamplesHelpFieldText"];
            var helpField = new EmbedFieldBuilder()
                .WithName("Need More Help?")
                .WithValue(helpFieldText);
            var examplesConfig = _Config["ExamplesText"];
            var newlineDelimiter = _Config["NewLineDelimiter"];
            //azure does not support new lines in app settings, so we have to do it in code
            var examplesDescription = examplesConfig.Replace(newlineDelimiter, "\n");
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(author)
                .WithDescription(examplesDescription)
                .AddField(helpField);
            await TryReplyAsync(embedBuilder.Build());
        }
        async Task ReplyWithJobConfig(JobConfigContainer config)
        {
            var embed = _TrendHelper.BuildEmbed(config, Context);
            await TryReplyAsync(embed);
        }
        async Task TryReplyAsync(string message) => await ReplyAsync(message: message, embed: null);
        async Task TryReplyAsync(Embed embed) => await ReplyAsync(message: string.Empty, embed: embed);
        protected override async Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            var warningResponses = _Config.Get<List<string>>("WarningResponses");
            try
            {
                return await base.ReplyAsync(message: message, embed: embed);
            }
            catch (HttpException httpException) when (warningResponses.Any(httpException.Message.Contains))
            {
                _Logger.LogWarning(httpException, $"Error replying to channel '{Context.Channel.Id}'. Deleting channel.");
                await _FunctionWrapper.DeleteJobConfigAsync(Context.Channel.Id);
                return null;
            }
        }
    }
}
