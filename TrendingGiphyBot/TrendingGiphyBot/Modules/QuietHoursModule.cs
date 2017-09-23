using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using NLog;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Helpers;

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
                    .WithAuthor(author)
                    .WithQuietHourFields(config, GlobalConfig.Config.HourOffset);
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
                var config = new JobConfig
                {
                    ChannelId = Context.Channel.Id,
                    MinQuietHour = ApplyHourOffset(minHour),
                    MaxQuietHour = ApplyHourOffset(maxHour)
                };
                await UpdateQuietHours(config);
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
                await UpdateQuietHours(config);
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        short ApplyHourOffset(short hour) => (short)((hour + GlobalConfig.Config.HourOffset) % 24);
        async Task UpdateJobConfig(JobConfig config)
        {
            await GlobalConfig.JobConfigDal.UpdateQuietHours(config);
            await Get();
        }
    }
}
