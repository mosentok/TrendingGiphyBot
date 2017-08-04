﻿using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using TrendingGiphyBot.Dals;
using NLog;
using System.Collections.Generic;
using System.Linq;
using GiphyDotNet.Model.Parameters;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        internal PostImageJob(IServiceProvider services, int interval, Time time) : base(services, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected override async Task Run()
        {
            if (await GlobalConfig.UrlCacheDal.Any())
            {
                var latestUrl = await GlobalConfig.UrlCacheDal.GetLatestUrl();
                var jobConfigs = await GlobalConfig.JobConfigDal.Get(Interval, Time);
                var jobConfigsNotInQuietHours = jobConfigs.Where(s => !s.IsInQuietHours()).ToList();
                var jobConfigsJustPostedTo = await PostChannelsNotInQuietHours(jobConfigsNotInQuietHours, latestUrl);
                var remainingJobConfigs = jobConfigsNotInQuietHours.Except(jobConfigsJustPostedTo).Where(s => s.RandomIsOn).ToList();
                var jobConfigsWithRandomStringOn = remainingJobConfigs.Where(s => !string.IsNullOrEmpty(s.RandomSearchString)).ToList();
                await PostChannelsWithRandomStringOn(jobConfigsWithRandomStringOn);
                var jobConfigsWithRandomStringOff = remainingJobConfigs.Except(jobConfigsWithRandomStringOn).ToList();
                await PostChannelsWithRandomStringOff(jobConfigsWithRandomStringOff);
            }
        }
        async Task<List<JobConfig>> PostChannelsNotInQuietHours(List<JobConfig> jobConfigsNotInQuietHours, string url)
        {
            var channelsPostedTo = new List<JobConfig>();
            foreach (var jobConfig in jobConfigsNotInQuietHours)
                if (!await GlobalConfig.UrlHistoryDal.Any(jobConfig.ChannelId, url)
                    && DiscordClient.GetChannel(jobConfig.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                {
                    await PostGif(jobConfig.ChannelId, url, $"*Trending!* {url}", socketTextChannel);
                    channelsPostedTo.Add(jobConfig);
                }
            return channelsPostedTo;
        }
        async Task PostChannelsWithRandomStringOn(List<JobConfig> jobConfigsWithRandomStringOn)
        {
            foreach (var jobConfig in jobConfigsWithRandomStringOn)
            {
                var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Rating = GlobalConfig.Ratings, Tag = jobConfig.RandomSearchString });
                await PostRandomGif(jobConfig, giphyRandomResult.Data.Url);
            }
        }
        async Task PostChannelsWithRandomStringOff(List<JobConfig> jobConfigsWithRandomStringOff)
        {
            var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Rating = GlobalConfig.Ratings });
            foreach (var jobConfig in jobConfigsWithRandomStringOff)
                await PostRandomGif(jobConfig, giphyRandomResult.Data.Url);
        }
        async Task PostRandomGif(JobConfig jobConfig, string url)
        {
            if (!await GlobalConfig.UrlHistoryDal.Any(jobConfig.ChannelId, url)
                && DiscordClient.GetChannel(jobConfig.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                await PostGif(jobConfig.ChannelId, url, url, socketTextChannel);
        }
        async Task PostGif(decimal channelId, string url, string message, SocketTextChannel socketTextChannel)
        {
            await Logger.SwallowAsync(async () =>
            {
                await socketTextChannel.SendMessageAsync(message);
                var history = new UrlHistory { ChannelId = channelId, Url = url };
                await GlobalConfig.UrlHistoryDal.Insert(history);
            });
        }
    }
}
