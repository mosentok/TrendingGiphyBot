using System;
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
        static readonly char[] _ArgsSplit = new[] { ' ' };
        public TrendModule(IServiceProvider services)
        {
            _Logger = LogManager.GetCurrentClassLogger();
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
            _Entities = _GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities();
            _TrendHelper = services.GetRequiredService<ITrendHelper>();
        }
        [Command(nameof(Get))]
        [Alias(nameof(Get), "", "Config", "Setup", "Help")]
        public async Task Get()
        {
            var isConfigured = await _Entities.AnyJobConfig(Context.Channel.Id);
            if (isConfigured)
                await GetJobConfig();
            else
                await NotConfiguredReplyAsync();
        }
        [Command(nameof(Off))]
        [Alias("Remove", "Stop")]
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
                    using (var entities = _GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                    {
                        await entities.UpdateInterval(Context.Channel.Id, interval, time);
                        await GetJobConfig();
                    }
                    return;
            }
        }
        [Command("Random")]
        public async Task TrendRandom([Remainder] string randomSearchString = null)
        {
            var isConfigured = await _Entities.AnyJobConfig(Context.Channel.Id);
            if (isConfigured)
                if (randomSearchString.Equals("off", StringComparison.CurrentCultureIgnoreCase))
                {
                    await _Entities.TurnOffRandom(Context.Channel.Id);
                    await GetJobConfig();
                }
                else
                {
                    var cleanedRandomSearchString = _TrendHelper.CleanRandomSearchString(randomSearchString);
                    var isValidRandomSearchString = _TrendHelper.IsValidRandomSearchString(cleanedRandomSearchString, _GlobalConfig.Config.RandomSearchStringMaxLength);
                    if (isValidRandomSearchString)
                    {
                        await _Entities.UpdateRandom(Context.Channel.Id, true, cleanedRandomSearchString);
                        await GetJobConfig();
                    }
                    else
                        await TryReplyAsync($"Random search string must be at most {_GlobalConfig.Config.RandomSearchStringMaxLength} characters long.");
                }
            else
                await NotConfiguredReplyAsync();
        }
        [Command(nameof(QuietHours))]
        public async Task QuietHours([Remainder] string quietHoursString = null)
        {
            var isConfigured = await _Entities.AnyJobConfig(Context.Channel.Id);
            if (isConfigured)
                if (!string.IsNullOrWhiteSpace(quietHoursString))
                    if (quietHoursString.Equals("off", StringComparison.CurrentCultureIgnoreCase))
                    {
                        await _Entities.TurnOffQuietHours(Context.Channel.Id);
                        await GetJobConfig();
                    }
                    else
                    {
                        var split = quietHoursString.Split(_ArgsSplit, StringSplitOptions.RemoveEmptyEntries);
                        if (split.Length == 2 && short.TryParse(split[0], out var minHour) && short.TryParse(split[1], out var maxHour))
                            using (var entities = _GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                            {
                                await entities.UpdateQuietHoursWithHourOffset(Context.Channel.Id, minHour, maxHour, _GlobalConfig.Config.HourOffset);
                                await GetJobConfig();
                            }
                        else
                            await HelpMessageReplyAsync();
                    }
                else
                    await HelpMessageReplyAsync();
            else
                await NotConfiguredReplyAsync();
        }
        [Command(nameof(Examples))]
        [Alias("Example")]
        public async Task Examples()
        {
            var config = await _Entities.GetJobConfigWithHourOffset(Context.Channel.Id, _GlobalConfig.Config.HourOffset);
            var guild = await Context.Client.GetGuildAsync(Context.Guild.Id);
            var author = new EmbedAuthorBuilder()
                .WithName($"Trending Giphy Bot Setup Examples")
                .WithIconUrl(guild.IconUrl);
            var field = new EmbedFieldBuilder()
                .WithName("Need More Help?")
                .WithValue("Visit [GitHub](https://github.com/mosentok/TrendingGiphyBot#commands) for more info.");
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(author)
                .WithDescription("```fix\n!trend every 10 minutes\n!trend random cats\n!trend quiethours 22 8\n!trend prefix ^```")
                .AddField(field);
            await TryReplyAsync(embedBuilder);
        }
        async Task GetJobConfig()
        {
            var config = await _Entities.GetJobConfigWithHourOffset(Context.Channel.Id, _GlobalConfig.Config.HourOffset);
            var guild = await Context.Client.GetGuildAsync(Context.Guild.Id);
            var author = new EmbedAuthorBuilder()
                .WithName($"Your Trending Giphy Bot Setup for Channel # {Context.Channel.Name}")
                .WithIconUrl(guild.IconUrl);
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(author)
                .AddInlineField(nameof(config.Interval), config.Interval)
                .AddInlineField(nameof(config.Time), config.Time.ToLower())
                .WithRandomConfigFields(config)
                .WithQuietHourFields(config, _GlobalConfig.Config.HourOffset)
                .AddField("Need Help?", "Type `!trend examples` for some examples, or visit [GitHub](https://github.com/mosentok/TrendingGiphyBot#commands) for more info.");
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
        public void Dispose()
        {
            _Entities?.Dispose();
        }
    }
}
