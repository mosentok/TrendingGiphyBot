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
                embedBuilder = AddQuietHour(embedBuilder, nameof(config.MinQuietHour), config.MinQuietHour);
                embedBuilder = AddQuietHour(embedBuilder, nameof(config.MaxQuietHour), config.MaxQuietHour);
                await ReplyAsync(string.Empty, embed: embedBuilder);
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        [Command(nameof(Set))]
        public async Task Set(params string[] searchString)
        {
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var config = new JobConfig
                {
                    ChannelId = Context.Channel.Id,
                    RandomIsOn = true,
                    RandomSearchString = string.Join(" ", searchString)
                };
                await UpdateJobs(config);
                await _GlobalConfig.JobConfigDal.UpdateQuietHours(config);
                await Get();
            }
            else
                await ReplyAsync(NotConfiguredMessage);
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
            var jobConfig = postImageJobs.SelectMany(s => s.JobConfigs).Single(s => s.ChannelId == Context.Channel.Id);
            jobConfig.MinQuietHour = config.MinQuietHour;
            jobConfig.MaxQuietHour = config.MaxQuietHour;
            return Task.CompletedTask;
        }
    }
}
