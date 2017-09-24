﻿using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using TrendingGiphyBot.Dals;
using NLog;
using System.Collections.Generic;
using System.Linq;
using Discord.Net;
using GiphyDotNet.Model.Parameters;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        internal PostImageJob(IGlobalConfig globalConfig, int interval, Time time) : base(globalConfig, LogManager.GetCurrentClassLogger(), interval, time) { }
        protected override async Task Run()
        {
            if (await GlobalConfig.UrlCacheDal.Any())
            {
                var latestUrls = await GlobalConfig.UrlCacheDal.GetLatestUrls();
                var jobConfigsNotInQuietHours = (await GetLiveJobConfigs()).Where(s => !s.IsInQuietHours()).ToList();
                var jobConfigsJustPostedTo = await PostChannelsNotInQuietHours(jobConfigsNotInQuietHours, latestUrls);
                var remainingJobConfigs = jobConfigsNotInQuietHours.Except(jobConfigsJustPostedTo).Where(s => s.RandomIsOn).ToList();
                var jobConfigsWithRandomStringOn = remainingJobConfigs.Where(s => !string.IsNullOrEmpty(s.RandomSearchString)).ToList();
                var jobConfigsWithRandomStringOff = remainingJobConfigs.Except(jobConfigsWithRandomStringOn).ToList();
                await PostChannelsWithRandomStringOn(jobConfigsWithRandomStringOn);
                await PostChannelsWithRandomStringOff(jobConfigsWithRandomStringOff);
            }
        }
        async Task<IEnumerable<JobConfig>> GetLiveJobConfigs()
        {
            var liveChannelIds = GlobalConfig.DiscordClient.Guilds.SelectMany(s => s.TextChannels).Select(s => s.Id).ToList();
            return (await GlobalConfig.JobConfigDal.Get(Interval, Time)).Where(s => liveChannelIds.Contains(s.ChannelId.ToULong()));
        }
        async Task<List<JobConfig>> PostChannelsNotInQuietHours(List<JobConfig> jobConfigsNotInQuietHours, List<string> urls)
        {
            var channelsPostedTo = new List<JobConfig>();
            foreach (var jobConfig in jobConfigsNotInQuietHours)
                if (DiscordClient.GetChannel(jobConfig.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                    await PostFirstNewGif(urls, channelsPostedTo, jobConfig, socketTextChannel);
            return channelsPostedTo;
        }
        async Task PostFirstNewGif(List<string> urls, List<JobConfig> channelsPostedTo, JobConfig jobConfig, SocketTextChannel socketTextChannel)
        {
            var url = await urls.FirstOrDefaultAsync(async s => !await GlobalConfig.UrlHistoryDal.Any(jobConfig.ChannelId, s));
            if (url != default)
            {
                await PostGif(jobConfig.ChannelId, url, $"*Trending!* {url}", socketTextChannel);
                channelsPostedTo.Add(jobConfig);
            }
        }
        async Task PostChannelsWithRandomStringOn(List<JobConfig> jobConfigsWithRandomStringOn)
        {
            foreach (var jobConfig in jobConfigsWithRandomStringOn)
                await Logger.SwallowAsync(async () =>
                {
                    var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Rating = GlobalConfig.Ratings, Tag = jobConfig.RandomSearchString });
                    await PostRandomGif(jobConfig, giphyRandomResult.Data.Url);
                });
        }
        async Task PostChannelsWithRandomStringOff(List<JobConfig> jobConfigsWithRandomStringOff)
        {
            if (jobConfigsWithRandomStringOff.Any())
                await Logger.SwallowAsync(async () =>
                {
                    var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Rating = GlobalConfig.Ratings });
                    foreach (var jobConfig in jobConfigsWithRandomStringOff)
                        await PostRandomGif(jobConfig, giphyRandomResult.Data.Url);
                });
        }
        async Task PostRandomGif(JobConfig jobConfig, string url)
        {
            if (!await GlobalConfig.UrlHistoryDal.Any(jobConfig.ChannelId, url)
                && DiscordClient.GetChannel(jobConfig.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                await PostGif(jobConfig.ChannelId, url, url, socketTextChannel);
        }
        async Task PostGif(decimal channelId, string url, string message, SocketTextChannel socketTextChannel)
        {
            try
            {
                await socketTextChannel.SendMessageAsync(message);
                var history = new UrlHistory { ChannelId = channelId, Url = url };
                await GlobalConfig.UrlHistoryDal.Insert(history);
            }
            catch (HttpException httpException)
            {
                Logger.Error(httpException.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
