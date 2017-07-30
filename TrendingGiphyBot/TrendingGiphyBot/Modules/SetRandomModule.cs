﻿using Discord;
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
    public class SetRandomModule : ModuleBase
    {
        const string _Name = "SetRandom";
        readonly IGlobalConfig _GlobalConfig;
        public SetRandomModule(IServiceProvider services)
        {
            _GlobalConfig = services.GetRequiredService<IGlobalConfig>();
        }
        string NotConfiguredMessage => $"{Context.Channel.Id} not configured. Configure me senpai! Use '{_GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)}' or '{_GlobalConfig.Config.DefaultPrefix}{nameof(JobConfig)} {nameof(Help)}' to learn how to.";
        [Command(nameof(Help))]
        [Alias(nameof(Help), "")]
        public async Task Help()
        {
            await ReplyAsync($"Visit {_GlobalConfig.Config.GitHubUrl} for help!");
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
                    .WithAuthor(author)
                    .AddInlineField(nameof(config.RandomIsOn), config.RandomIsOn);
                EmbedBuilder embed;
                if (!string.IsNullOrEmpty(config.RandomSearchString))
                    embed = embedBuilder
                        .AddInlineField(nameof(config.RandomSearchString), config.RandomSearchString);
                else
                    embed = embedBuilder
                        .AddInlineField(nameof(config.RandomSearchString), "null");
                await ReplyAsync(string.Empty, embed: embed);
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        [Command(nameof(On))]
        public async Task On(params string[] searchValues)
        {
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var randomSearchString = string.Join(" ", searchValues);
                if (randomSearchString.Length <= _GlobalConfig.Config.RandomSearchStringMaxLength)
                {
                    var config = new JobConfig
                    {
                        ChannelId = Context.Channel.Id,
                        RandomIsOn = true,
                        RandomSearchString = randomSearchString
                    };
                    await UpdateJobs(config);
                    await _GlobalConfig.JobConfigDal.UpdateRandom(config);
                    await Get();
                }
                else
                    await ReplyAsync($"Random search string must be at most {_GlobalConfig.Config.RandomSearchStringMaxLength} characters long.");
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        [Command(nameof(Off))]
        public async Task Off()
        {
            if (await _GlobalConfig.JobConfigDal.Any(Context.Channel.Id))
            {
                var config = new JobConfig { ChannelId = Context.Channel.Id, RandomIsOn = false };
                await UpdateJobs(config);
                await _GlobalConfig.JobConfigDal.UpdateRandom(config);
                await ReplyAsync("No more Randies.");
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        Task UpdateJobs(JobConfig config)
        {
            //TODO centralize random set
            var postImageJobs = _GlobalConfig.Jobs.OfType<PostImageJob>().ToList();
            var jobConfig = postImageJobs.SelectMany(s => s.JobConfigs).Single(s => s.ChannelId == Context.Channel.Id);
            jobConfig.RandomIsOn = config.RandomIsOn;
            jobConfig.RandomSearchString = config.RandomSearchString;
            return Task.CompletedTask;
        }
    }
}
