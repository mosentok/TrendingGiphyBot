﻿using System.Threading.Tasks;
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

namespace TrendingGiphyBot.Modules
{
    [Group(nameof(JobConfig))]
    public class JobConfigModule : ModuleBase
    {
        List<Job> _Jobs => (Context as JobConfigCommandContext).Jobs;
        JobConfigDal _ChannelJobConfigDal => (Context as JobConfigCommandContext).ChannelJobConfigDal;
        Giphy _GiphyClient => (Context as JobConfigCommandContext).GiphyClient;
        int MinimumMinutes => (Context as JobConfigCommandContext).MinimumMinutes;
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
            var any = await _ChannelJobConfigDal.Any(Context.Channel.Id);
            if (any)
            {
                var config = await _ChannelJobConfigDal.Get(Context.Channel.Id);
                var serialized = JsonConvert.SerializeObject(config, Formatting.Indented);
                await ReplyAsync(serialized);
            }
            else
                await ReplyAsync($"{Context.Channel.Id} not configured.");
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
        async Task SaveConfig(int interval, Time time)
        {
            var config = new JobConfig
            {
                ChannelId = Context.Channel.Id,
                Interval = interval,
                Time = time.ToString()
            };
            var any = await _ChannelJobConfigDal.Any(Context.Channel.Id);
            if (any)
                await _ChannelJobConfigDal.Update(config);
            else
            {
                await _ChannelJobConfigDal.Insert(config);
                _Jobs.Add(new Job(_GiphyClient, Context.Client as DiscordSocketClient, config));
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
