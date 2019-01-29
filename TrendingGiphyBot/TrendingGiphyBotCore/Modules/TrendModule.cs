using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrendingGiphyBotCore.Configuration;
using TrendingGiphyBotCore.Enums;
using TrendingGiphyBotCore.Exceptions;
using TrendingGiphyBotCore.Extensions;
using TrendingGiphyBotCore.Helpers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotCore.Modules
{
    [Group("Trend")]
    public class TrendModule : ModuleBase
    {
        readonly ILogger _Logger;
        readonly ITrendHelper _TrendHelper;
        readonly IFunctionHelper _FunctionHelper;
        readonly IConfiguration _Config;
        readonly List<short> _ValidMinutes;
        readonly List<short> _ValidHours;
        readonly SubJobConfig _MinJobConfig;
        readonly SubJobConfig _MaxJobConfig;
        static readonly char[] _ArgsSplit = new[] { ' ' };
        public TrendModule(IServiceProvider services)
        {
            _Logger = services.GetService<ILogger<TrendModule>>();
            _TrendHelper = services.GetRequiredService<ITrendHelper>();
            _FunctionHelper = services.GetRequiredService<IFunctionHelper>();
            _Config = services.GetService<IConfiguration>();
            var validMinutesString = _Config["ValidMinutes"];
            _ValidMinutes = validMinutesString.Split(',').Select(short.Parse).ToList();
            var validHoursString = _Config["ValidHours"];
            _ValidHours = validHoursString.Split(',').Select(short.Parse).ToList();
            var minInterval = _Config.GetValue<short>("MinInterval");
            var minTime = _Config.GetValue<Time>("MinTime");
            _MinJobConfig = new SubJobConfig(minInterval, minTime);
            var maxInterval = _Config.GetValue<short>("MaxInterval");
            var maxTime = _Config.GetValue<Time>("MaxTime");
            _MaxJobConfig = new SubJobConfig(maxInterval, maxTime);
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
            await _FunctionHelper.DeleteJobConfigAsync(Context.Channel.Id);
            await TryReplyAsync("Done.");
        }
        [Command(nameof(Every))]
        public async Task Every(short interval, Time time)
        {
            var state = DetermineJobConfigState(interval, time);
            await ProcessJobConfigRequest(state, interval, time);
        }
        Task ProcessJobConfigRequest(JobConfigState state, short interval, Time time)
        {
            switch (state)
            {
                case JobConfigState.InvalidHours:
                    var invalidHoursMessage = _TrendHelper.InvalidConfigMessage(time, _ValidHours);
                    return TryReplyAsync(invalidHoursMessage);
                case JobConfigState.InvalidMinutes:
                    var invalidMinutesMessage = _TrendHelper.InvalidConfigMessage(time, _ValidMinutes);
                    return TryReplyAsync(invalidMinutesMessage);
                case JobConfigState.InvalidTime:
                    return TryReplyAsync($"{time.ToString()} is an invalid {nameof(Time)}.");
                case JobConfigState.IntervalTooSmall:
                case JobConfigState.IntervallTooBig:
                    var invalidConfigRangeMessage = _TrendHelper.InvalidConfigRangeMessage(_MinJobConfig, _MaxJobConfig);
                    return TryReplyAsync(invalidConfigRangeMessage);
                case JobConfigState.Valid:
                    return SetJobConfig(interval, time);
                default:
                    throw new UnexpectedTimeException(time);
            }
        }
        async Task SetJobConfig(short interval, Time time)
        {
            var match = await _FunctionHelper.GetJobConfigAsync(Context.Channel.Id);
            var container = new JobConfigContainer(match, interval, time.ToString());
            var result = await _FunctionHelper.PostJobConfigAsync(Context.Channel.Id, container);
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
            var cleanedRandomSearchString = _TrendHelper.CleanRandomSearchString(randomSearchString);
            var maxLengthString = _Config["RandomSearchStringMaxLength"];
            var maxLength = Convert.ToInt32(maxLengthString);
            var isValidRandomSearchString = _TrendHelper.IsValidRandomSearchString(cleanedRandomSearchString, maxLength);
            if (!isValidRandomSearchString)
                return TryReplyAsync($"Random search string must be at most {maxLengthString} characters long.");
            var shouldTurnCommandOff = _TrendHelper.ShouldTurnCommandOff(randomSearchString);
            if (shouldTurnCommandOff)
                return SetRandom(match, null);
            return SetRandom(match, cleanedRandomSearchString);
        }
        async Task SetRandom(JobConfigContainer match, string randomSearchString)
        {
            var container = new JobConfigContainer(match, randomSearchString);
            var result = await _FunctionHelper.PostJobConfigAsync(Context.Channel.Id, container);
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
            var result = await _FunctionHelper.PostJobConfigAsync(Context.Channel.Id, container);
            await ReplyWithJobConfig(result);
        }
        [Command(nameof(Prefix))]
        public async Task Prefix(string prefix)
        {
            await ProcessPrefixRequest(prefix);
        }
        Task ProcessPrefixRequest(string prefix)
        {
            if (string.IsNullOrEmpty(prefix) || prefix.Length > 4)
                return TryReplyAsync("Prefix must be 1-4 characters long.");
            var newPrefix = DetermineNewPrefix(prefix);
            return _FunctionHelper.PostPrefixAsync(Context.Channel.Id, newPrefix)
                .ContinueWith(t => TryReplyAsync($"New prefix: {t.Result}"))
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
        public async Task Examples() => await ExamplesReplyAsync();
        async Task ExamplesReplyAsync()
        {
            var author = new EmbedAuthorBuilder()
                .WithName("Trending Giphy Bot Examples")
                .WithIconUrl(Context.Guild.IconUrl);
            var helpFieldText = _Config["ExamplesHelpFieldText"];
            var helpField = new EmbedFieldBuilder()
                .WithName("Need More Help?")
                .WithValue(helpFieldText);
            var examplesDescription = _Config["ExamplesText"];
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(author)
                .WithDescription(examplesDescription)
                .AddField(helpField);
            await TryReplyAsync(embedBuilder);
        }
        async Task ReplyWithJobConfig(JobConfigContainer config)
        {
            var author = new EmbedAuthorBuilder()
                .WithName($"Setup for Channel # {Context.Channel.Name}")
                .WithIconUrl(Context.Guild.IconUrl);
            var helpFieldText = _Config["GetConfigHelpFieldText"];
            var helpField = new EmbedFieldBuilder()
                .WithName("Need Help?")
                .WithValue(helpFieldText);
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(author)
                .WithHowOften(config)
                .WithRandomConfigFields(config)
                .WithQuietHourFields(config)
                .AddField(helpField);
            await TryReplyAsync(embedBuilder);
        }
        async Task TryReplyAsync(string message) => await TryReplyAsync(message, null);
        async Task TryReplyAsync(EmbedBuilder embedBuilder) => await TryReplyAsync(string.Empty, embedBuilder);
        async Task TryReplyAsync(string message, EmbedBuilder embedBuilder)
        {
            try
            {
                await ReplyAsync(message, embed: embedBuilder.Build());
            }
            //TODO move these to config
            catch (HttpException httpException) when (httpException.Message.EndsWith("Missing Access") ||
                                                      httpException.Message.EndsWith("Missing Permissions"))
            {
                _Logger.LogWarning(httpException.Message);
            }
        }
        JobConfigState DetermineJobConfigState(short interval, Time time)
        {
            var minTimeSpan = AsTimeSpan(_MinJobConfig);
            var maxTimeSpan = AsTimeSpan(_MaxJobConfig);
            var desiredTimeSpan = AsTimeSpan(interval, time);
            if (desiredTimeSpan >= minTimeSpan)
            {
                if (desiredTimeSpan <= maxTimeSpan)
                    return DetermineTimeState(interval, time);
                return JobConfigState.IntervallTooBig;
            }
            return JobConfigState.IntervalTooSmall;
        }
        JobConfigState DetermineTimeState(short interval, Time time)
        {
            switch (time)
            {
                case Time.Hour:
                case Time.Hours:
                    if (_ValidHours.Contains(interval))
                        return JobConfigState.Valid;
                    return JobConfigState.InvalidHours;
                case Time.Minute:
                case Time.Minutes:
                    if (_ValidMinutes.Contains(interval))
                        return JobConfigState.Valid;
                    return JobConfigState.InvalidMinutes;
                default:
                    return JobConfigState.InvalidTime;
            }
        }
        static TimeSpan AsTimeSpan(SubJobConfig config) => AsTimeSpan(config.Interval, config.Time);
        static TimeSpan AsTimeSpan(short interval, Time time)
        {
            switch (time)
            {
                case Time.Hour:
                case Time.Hours:
                    return TimeSpan.FromHours(interval);
                case Time.Minute:
                case Time.Minutes:
                    return TimeSpan.FromMinutes(interval);
                default:
                    throw new UnexpectedTimeException(time);
            }
        }
    }
}
