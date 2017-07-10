using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Commands;
using System.Collections.Generic;
using GiphyDotNet.Manager;
using Discord.WebSocket;
using System;
using TrendingGiphyBot.Exceptions;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.CommandContexts;
using TrendingGiphyBot.Dals;
using TrendingGiphyBot.Helpers;
using TrendingGiphyBot.Jobs;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(JobConfig))]
    public class JobConfigModule : ModuleBase
    {
        string NotConfiguredMessage => $"{Context.Channel.Id} not configured. Configure me senpai! Use '!JobConfig' or '!JobConfig Help' to learn how to.";
        List<Job> _Jobs => (Context as JobConfigCommandContext).Jobs;
        JobConfigDal _JobConfigDal => (Context as JobConfigCommandContext).ChannelJobConfigDal;
        Giphy _GiphyClient => (Context as JobConfigCommandContext).GiphyClient;
        int MinimumMinutes => (Context as JobConfigCommandContext).MinimumMinutes;
        string WordnikToken => (Context as JobConfigCommandContext).WordnikToken;
        [Command]
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
            var isValid = IsValid(interval, time);
            if (isValid)
            {
                await SaveConfig(interval, time);
                await Get();
            }
            else
                await ReplyAsync($"{nameof(JobConfig.Interval)} and {nameof(JobConfig.Time)} must combine to at least {MinimumMinutes} minutes.");
        }
        [Command(nameof(Remove))]
        [Summary("Removes the " + nameof(JobConfig) + " for this channel.")]
        public async Task Remove()
        {
            if (await _JobConfigDal.Any(Context.Channel.Id))
            {
                if (!string.IsNullOrEmpty(WordnikToken))
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var url = "http://api.wordnik.com/v4/words.json/wordOfTheDay/definitions?api_key=" + WordnikToken;
                        var response = await client.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            var stringResponse = await response.Content.ReadAsStringAsync();
                        }
                    }
                await ReplyAsync("Configuration removed.");
                await _JobConfigDal.Remove(Context.Channel.Id);
            }
            else
                await ReplyAsync(NotConfiguredMessage);
        }
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
                await _JobConfigDal.Update(config);
            else
            {
                await _JobConfigDal.Insert(config);
                _Jobs.Add(new PostImageJob(_GiphyClient, Context.Client as DiscordSocketClient, config, _JobConfigDal));
            }
        }
        bool IsValid(int interval, Time time)
        {
            var configgedMinutes = DetermineJobIntervalSeconds(interval, time);
            return configgedMinutes >= MinimumMinutes;
        }
        static double DetermineJobIntervalSeconds(int interval, Time time)
        {
            switch (time)
            {
                case Time.Hours:
                    return TimeSpan.FromHours(interval).TotalMinutes;
                case Time.Minutes:
                    return TimeSpan.FromMinutes(interval).TotalMinutes;
                case Time.Seconds:
                    return TimeSpan.FromSeconds(interval).TotalMinutes;
                default:
                    throw new InvalidTimeException(time);
            }
        }
    }
}
