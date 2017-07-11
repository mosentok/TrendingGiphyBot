using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Commands;
using System.Collections.Generic;
using GiphyDotNet.Manager;
using Discord.WebSocket;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.CommandContexts;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Helpers;
using TrendingGiphyBot.Jobs;
using System.Linq;
using TrendingGiphyBot.Wordnik.Clients;

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(JobConfig))]
    public class JobConfigModule : ModuleBase
    {
        List<Job> _Jobs;
        JobConfigDal _JobConfigDal;
        UrlCacheDal _UrlCacheDal;
        Giphy _GiphyClient;
        int _MinimumMinutes;
        WordnikClient _WordnikClient;
        bool initialized;
        protected override void BeforeExecute(CommandInfo command)
        {
            if (!initialized)
            {
                var context = Context as JobConfigCommandContext;
                _Jobs = context.Jobs;
                _JobConfigDal = context.ChannelJobConfigDal;
                _UrlCacheDal = context.UrlCacheDal;
                _GiphyClient = context.GiphyClient;
                _MinimumMinutes = context.MinimumMinutes;
                _WordnikClient = context.WordnikClient;
                initialized = true;
            }
        }
        string NotConfiguredMessage => $"{Context.Channel.Id} not configured. Configure me senpai! Use '!{nameof(JobConfig)}' or '!{nameof(JobConfig)} {nameof(Help)}' to learn how to.";
        //TODO this is really ugly fam
        [Command(nameof(Help))]
        [Summary("Help menu for the " + nameof(JobConfig) + " commands.")]
        [Alias(nameof(Help))]
        public async Task Help()
        {
            var helpContainer = ModuleBaseHelper.BuildHelpContainer<JobConfigModule>();
            var serialized = JsonConvert.SerializeObject(helpContainer, Formatting.Indented);
            await ReplyAsync(serialized);
        }
        [Command(nameof(Get))]
        [Summary("Gets the " + nameof(JobConfig) + " for this channel.")]
        public async Task Get()
        {
            var any = await _JobConfigDal.Any(Context.Channel.Id);
            if (any)
            {
                var config = await _JobConfigDal.Get(Context.Channel.Id);
                var serialized = JsonConvert.SerializeObject(config, Formatting.Indented);
                await ReplyAsync(serialized);
            }
            else
                await base.ReplyAsync(NotConfiguredMessage);
        }
        [Command(nameof(Set))]
        [Summary("Sets the " + nameof(JobConfig) + " for this channel.")]
        public async Task Set(
            [Summary(nameof(JobConfig.Interval) + " to set.")]
            int interval,
            [Summary(nameof(JobConfig.Time) + " to set.")]
            Time time)
        {
            var isValid = ModuleBaseHelper.IsValid(interval, time, _MinimumMinutes);
            if (isValid)
            {
                await SaveConfig(interval, time);
                await Get();
            }
            else
                await ReplyAsync($"{nameof(JobConfig.Interval)} and {nameof(JobConfig.Time)} must combine to at least {_MinimumMinutes} minutes.");
        }
        [Command(nameof(Remove))]
        [Summary("Removes the " + nameof(JobConfig) + " for this channel.")]
        public async Task Remove()
        {
            if (await _JobConfigDal.Any(Context.Channel.Id))
            {
                await SendRemoveMessage();
                await _JobConfigDal.Remove(Context.Channel.Id);
                var toRemove = _Jobs.OfType<PostImageJob>().Single(s => s.ChannelId == Context.Channel.Id);
                _Jobs.Remove(toRemove);
                toRemove?.Dispose();
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
        async Task SendRemoveMessage()
        {
            var wordOfTheDay = await _WordnikClient?.GetWordOfTheDay();
            if (wordOfTheDay == null)
                await ReplyAsync("Configuration removed.");
            else
                await base.ReplyAsync($"Configuration removed. {CapitalizeFirstLetter(wordOfTheDay.Word)}.");
        }
        static string CapitalizeFirstLetter(string s) => char.ToUpper(s[0]) + s.Substring(1);
        async Task SaveConfig(int interval, Time time)
        {
            var config = new JobConfig
            {
                ChannelId = Context.Channel.Id,
                Interval = interval,
                Time = time.ToString()
            };
            var any = await _JobConfigDal.Any(Context.Channel.Id);
            if (any)
            {
                await _JobConfigDal.Update(config);
                _Jobs.OfType<PostImageJob>().Single(s => s.ChannelId == Context.Channel.Id).Restart(config);
            }
            else
            {
                await _JobConfigDal.Insert(config);
                var postImageJob = new PostImageJob(_GiphyClient, Context.Client as DiscordSocketClient, config, _JobConfigDal, _UrlCacheDal);
                _Jobs.Add(postImageJob);
                postImageJob.StartTimerWithCloseInterval();
            }
        }
    }
}
