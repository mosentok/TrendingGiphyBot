using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using NLog;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Extensions;

namespace TrendingGiphyBot.Modules
{
    [Group(_Name)]
    public class QuietHoursModule : BotModuleBase
    {
        const string _Name = "QuietHours";
        public QuietHoursModule(IServiceProvider services) : base(services, LogManager.GetCurrentClassLogger()) { }
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
        public async Task Set(short minHour, short maxHour)
        {
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                if (await entities.AnyJobConfig(Context.Channel.Id))
                {
                    var config = new JobConfig
                    {
                        ChannelId = Context.Channel.Id,
                        MinQuietHour = ApplyHourOffset(minHour),
                        MaxQuietHour = ApplyHourOffset(maxHour)
                    };
                    await UpdateJobConfig(config, entities);
                }
                else
                    await TryReplyAsync(NotConfiguredMessage);
        }
        [Command(nameof(Reset))]
        public async Task Reset()
        {
            using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                if (await entities.AnyJobConfig(Context.Channel.Id))
                {
                    var config = new JobConfig
                    {
                        ChannelId = Context.Channel.Id,
                        MinQuietHour = null,
                        MaxQuietHour = null
                    };
                    await UpdateJobConfig(config, entities);
                }
                else
                    await TryReplyAsync(NotConfiguredMessage);
        }
        short ApplyHourOffset(short hour) => (short)((hour + GlobalConfig.Config.HourOffset) % 24);
        async Task UpdateJobConfig(JobConfig config, TrendingGiphyBotEntities entities)
        {
            await entities.UpdateQuietHours(config);
            await Get(entities);
        }
        async Task Get(TrendingGiphyBotEntities entities)
        {
            if (await entities.AnyJobConfig(Context.Channel.Id))
            {
                var config = await entities.GetJobConfig(Context.Channel.Id);
                var avatarUrl = (await Context.Client.GetGuildAsync(Context.Guild.Id)).IconUrl;
                var author = new EmbedAuthorBuilder()
                    .WithName($"{Context.Channel.Name}'s {_Name}")
                    .WithIconUrl(avatarUrl);
                var embedBuilder = new EmbedBuilder()
                    .WithAuthor(author)
                    .WithQuietHourFields(config, GlobalConfig.Config.HourOffset);
                await TryReplyAsync(embedBuilder);
            }
            else
                await TryReplyAsync(NotConfiguredMessage);
        }
    }
}
