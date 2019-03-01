using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotCore.Enums;
using TrendingGiphyBotCore.Exceptions;
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
            var state = _TrendHelper.DetermineJobConfigState(interval, time);
            await ProcessJobConfigRequest(state, interval, time);
        }
        Task ProcessJobConfigRequest(JobConfigState state, short interval, Time time)
        {
            switch (state)
            {
                case JobConfigState.InvalidHours:
                    var invalidHoursMessage = _TrendHelper.InvalidHoursConfigMessage(time);
                    return TryReplyAsync(invalidHoursMessage);
                case JobConfigState.InvalidMinutes:
                    var invalidMinutesMessage = _TrendHelper.InvalidMinutesConfigMessage(time);
                    return TryReplyAsync(invalidMinutesMessage);
                case JobConfigState.InvalidTime:
                    return TryReplyAsync($"{time.ToString()} is an invalid {nameof(Time)}.");
                case JobConfigState.IntervalTooSmall:
                case JobConfigState.IntervallTooBig:
                    var invalidConfigRangeMessage = _TrendHelper.InvalidConfigRangeMessage();
                    return TryReplyAsync(invalidConfigRangeMessage);
                case JobConfigState.Valid:
                    return SetJobConfig(interval, time);
                default:
                    throw new UnexpectedTimeException(time);
            }
        }
        async Task SetJobConfig(short interval, Time time)
        {
            var match = await _FunctionWrapper.GetJobConfigAsync(Context.Channel.Id);
            var container = new JobConfigContainer(match, interval, time.ToString());
            var result = await _FunctionWrapper.PostJobConfigAsync(Context.Channel.Id, container);
            await ReplyWithJobConfig(result);
        }
        [Command("Random")]
        public async Task TrendRandom([Remainder] string randomSearchString)
        {
            var match = await _FunctionWrapper.GetJobConfigAsync(Context.Channel.Id);
            await ProcessRandomRequest(randomSearchString, match);
        }
        Task ProcessRandomRequest(string randomSearchString, JobConfigContainer match)
        {
            var maxLength = _Config.GetValue<int>("RandomSearchStringMaxLength");
            var isValidSearchString = !string.IsNullOrEmpty(randomSearchString) && randomSearchString.Length <= maxLength;
            if (!isValidSearchString)
                return TryReplyAsync($"Please provide a random gif filter that is at most {maxLength} characters long.");
            var shouldTurnCommandOff = _TrendHelper.ShouldTurnCommandOff(randomSearchString);
            if (shouldTurnCommandOff)
                return SetRandom(match, null);
            return SetRandom(match, randomSearchString);
        }
        async Task SetRandom(JobConfigContainer match, string randomSearchString)
        {
            var container = new JobConfigContainer(match, randomSearchString);
            var result = await _FunctionWrapper.PostJobConfigAsync(Context.Channel.Id, container);
            await ReplyWithJobConfig(result);
        }
        [Command(nameof(Between))]
        public async Task Between([Remainder] string trendBetweenString)
        {
            var match = await _FunctionWrapper.GetJobConfigAsync(Context.Channel.Id);
            await ProcessBetweenRequest(trendBetweenString, match);
        }
        Task ProcessBetweenRequest(string trendBetweenString, JobConfigContainer match)
        {
            var shouldTurnCommandOff = _TrendHelper.ShouldTurnCommandOff(trendBetweenString);
            if (shouldTurnCommandOff)
                return SetBetween(match, null, null);
            var split = trendBetweenString.Split(_ArgsSplit, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 3)
                return TryReplyAsync(_Config["InvalidQuietHoursInputFormat"]);
            var minInRange = _TrendHelper.IsInRange(split[2], out var minQuietHour);
            var maxInRange = _TrendHelper.IsInRange(split[0], out var maxQuietHour);
            if (!(minInRange && maxInRange) || minQuietHour == maxQuietHour)
                return TryReplyAsync(_Config["InvalidQuietHoursRangeMessage"]);
            return SetBetween(match, minQuietHour, maxQuietHour);
        }
        async Task SetBetween(JobConfigContainer match, short? minQuietHour, short? maxQuietHour)
        {
            var container = new JobConfigContainer(match, minQuietHour, maxQuietHour);
            var result = await _FunctionWrapper.PostJobConfigAsync(Context.Channel.Id, container);
            await ReplyWithJobConfig(result);
        }
        [Command(nameof(Prefix))]
        public async Task Prefix(string prefix)
        {
            var match = await _FunctionWrapper.GetJobConfigAsync(Context.Channel.Id);
            await ProcessPrefixRequest(prefix, match);
            _TrendHelper.OnPrefixUpdated(match.ChannelId, prefix);
        }
        Task ProcessPrefixRequest(string prefix, JobConfigContainer match)
        {
            if (string.IsNullOrEmpty(prefix) || prefix.Length > 4)
                return TryReplyAsync("Prefix must be 1-4 characters long.");
            var newPrefix = DetermineNewPrefix(prefix);
            match.Prefix = newPrefix;
            return _FunctionWrapper.PostJobConfigAsync(Context.Channel.Id, match)
                .ContinueWith(t => TryReplyAsync($"New prefix: {t.Result.Prefix}"))
                .Unwrap();
        }
        string DetermineNewPrefix(string prefix)
        {
            if (_TrendHelper.ShouldTurnCommandOff(prefix))
                return _Config["DefaultPrefix"];
            return prefix;
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
                _Logger.LogWarning(httpException.Message);
                return null;
            }
        }
    }
}
