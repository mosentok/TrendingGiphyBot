using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using NLog;
using TrendingGiphyBot.Dals;

namespace TrendingGiphyBot.Modules
{
    [Group(_Name)]
    public class QuietHoursModule : LoggingModuleBase
    {
        const string _Name = "QuietHours";
        public QuietHoursModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()) { }
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
            if (await GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var config = await GlobalConfig.JobConfigDal.Get(Context.Channel.Id);
                var avatarUrl = (await Context.Client.GetGuildAsync(Context.Guild.Id)).IconUrl;
                var author = new EmbedAuthorBuilder()
                    .WithName($"{Context.Channel.Name}'s {_Name}")
                    .WithIconUrl(avatarUrl);
                var embedBuilder = new EmbedBuilder()
                    .WithAuthor(author);
                var minQuietHour = UndoHourOffset(config.MinQuietHour);
                var maxQuietHour = UndoHourOffset(config.MaxQuietHour);
                embedBuilder = AddQuietHour(embedBuilder, nameof(config.MinQuietHour), minQuietHour);
                embedBuilder = AddQuietHour(embedBuilder, nameof(config.MaxQuietHour), maxQuietHour);
                await ReplyAsync(string.Empty, embed: embedBuilder);
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        [Command(nameof(Set))]
        public async Task Set(short minHour, short maxHour)
        {
            if (await GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var minQuietHour = ApplyHourOffset(minHour);
                var maxQuietHour = ApplyHourOffset(maxHour);
                var config = new JobConfig
                {
                    ChannelId = Context.Channel.Id,
                    MinQuietHour = minQuietHour,
                    MaxQuietHour = maxQuietHour
                };
                await UpdateJobConfig(config);
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        [Command(nameof(Reset))]
        public async Task Reset()
        {
            if (await GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var config = new JobConfig
                {
                    ChannelId = Context.Channel.Id,
                    MinQuietHour = null,
                    MaxQuietHour = null
                };
                await UpdateJobConfig(config);
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        short? UndoHourOffset(short? hour)
        {
            if (hour.HasValue)
            {
                var newHour = hour - GlobalConfig.Config.HourOffset;
                if (newHour >= 24)
                    newHour = newHour - 24;
                else if (newHour < 0)
                    newHour = newHour + 24;
                return (short)newHour;
            }
            return null;
        }
        short ApplyHourOffset(short hour)
        {
            var newHour = hour + GlobalConfig.Config.HourOffset;
            if (newHour >= 24)
                newHour = newHour - 24;
            else if (newHour < 0)
                newHour = newHour + 24;
            return (short)newHour;
        }
        async Task UpdateJobConfig(JobConfig config)
        {
            await GlobalConfig.JobConfigDal.UpdateQuietHours(config);
            await Get();
        }
        static EmbedBuilder AddQuietHour(EmbedBuilder embedBuilder, string name, short? quietHour)
        {
            if (quietHour.HasValue)
                return embedBuilder
                    .AddInlineField(name, quietHour.Value);
            return embedBuilder
                .AddInlineField(name, "null");
        }
    }
}
