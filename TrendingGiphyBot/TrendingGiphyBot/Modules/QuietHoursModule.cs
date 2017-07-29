using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Jobs;

namespace TrendingGiphyBot.Modules
{
    [Group(_Name)]
    public class QuietHoursModule : ModuleBase
    {
        const string _Name = "QuietHours";
        IGlobalConfig _GlobalConfig;
        public QuietHoursModule(IServiceProvider services)
        {
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        string NotConfiguredMessage => $"{Context.Channel.Id} not configured. Configure me senpai! Use '{_GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)}' or '{_GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)} {nameof(Help)}' to learn how to.";
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help()
        {
            await ReplyAsync($"Visit {_GlobalConfig.Config.GitHubUrl} for help!");
            //await SendHelpMenu();
        }
        [Command(nameof(Get))]
        public async Task Get()
        {
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var config = await _GlobalConfig.JobConfigDal.Get(Context.Channel.Id);
                var avatarUrl = (await Context.Client.GetGuildAsync(Context.Guild.Id)).IconUrl;
                var author = new EmbedAuthorBuilder()
                    .WithName($"{Context.Channel.Name}'s {_Name}")
                    .WithIconUrl(avatarUrl);
                var embedBuilder = new EmbedBuilder()
                    .WithAuthor(author);
                var minQuietHour = ReverseHourOffset(config.MinQuietHour);
                var maxQuietHour = ReverseHourOffset(config.MaxQuietHour);
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
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
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
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
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
        short? ReverseHourOffset(short? hour)
        {
            if (hour.HasValue)
            {
                var newHour = hour - _GlobalConfig.Config.HourOffset;
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
            var newHour = hour + _GlobalConfig.Config.HourOffset;
            if (newHour >= 24)
                newHour = newHour - 24;
            else if (newHour < 0)
                newHour = newHour + 24;
            return (short)newHour;
        }
        async Task UpdateJobConfig(JobConfig config)
        {
            await UpdateJobs(config);
            await _GlobalConfig.JobConfigDal.UpdateQuietHours(config);
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
        Task UpdateJobs(JobConfig config)
        {
            //TODO centralize min/max quiet hours set
            var postImageJobs = _GlobalConfig.Jobs.OfType<PostImageJob>().ToList();
            var configToUpdate = postImageJobs.SelectMany(s => s.JobConfigs).Single(s => s.ChannelId == Context.Channel.Id);
            configToUpdate.MinQuietHour = config.MinQuietHour;
            configToUpdate.MaxQuietHour = config.MaxQuietHour;
            return Task.CompletedTask;
        }
    }
}
