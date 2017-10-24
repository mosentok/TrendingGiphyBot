﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Discord.WebSocket;
using TrendingGiphyBot.Dals;
using NLog;
using System.Collections.Generic;
using System.Linq;
using Discord.Net;
using GiphyDotNet.Model.Parameters;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Extensions;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        internal PostImageJob(IGlobalConfig globalConfig, SubJobConfig subJobConfig) : base(globalConfig, LogManager.GetCurrentClassLogger(), subJobConfig) { }
        protected override async Task Run()
        {
            if (await GlobalConfig.UrlCacheDal.Any())
            {
                var jobConfigsNotInQuietHours = (await GetLiveJobConfigs()).Where(s => !s.IsInQuietHours()).ToList();
                var jobConfigsJustPostedTo = await PostChannelsNotInQuietHours(jobConfigsNotInQuietHours);
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
        async Task<ConcurrentBag<JobConfig>> PostChannelsNotInQuietHours(List<JobConfig> jobConfigsNotInQuietHours)
        {
            var channelsPostedTo = new ConcurrentBag<JobConfig>();
            var tasks = new List<Task>();
            foreach (var jobConfig in jobConfigsNotInQuietHours)
                if (DiscordClient.GetChannel(jobConfig.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                    tasks.Add(PostFirstNewGif(channelsPostedTo, jobConfig, socketTextChannel));
            await Task.WhenAll(tasks);
            return channelsPostedTo;
        }
        async Task PostFirstNewGif(ConcurrentBag<JobConfig> channelsPostedTo, JobConfig jobConfig, SocketTextChannel socketTextChannel)
        {
            var url = await GlobalConfig.LatestUrls.OrderByDescending(s => s.Stamp).Select(s => s.Url).FirstOrDefaultAsync(async s => !await GlobalConfig.UrlHistoryDal.Any(jobConfig.ChannelId, s));
            if (url != default)
            {
                await PostGif(jobConfig.ChannelId, url, $"*Trending!* {url}", socketTextChannel);
                channelsPostedTo.Add(jobConfig);
            }
        }
        async Task PostChannelsWithRandomStringOn(List<JobConfig> jobConfigsWithRandomStringOn)
        {
            var tasks = jobConfigsWithRandomStringOn.Select(jobConfig => Logger.SwallowAsync(async () =>
            {
                var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Rating = GlobalConfig.Ratings, Tag = jobConfig.RandomSearchString });
                await PostRandomGif(jobConfig, giphyRandomResult.Data.Url);
            })).ToList();
            await Task.WhenAll(tasks);
        }
        async Task PostChannelsWithRandomStringOff(List<JobConfig> jobConfigsWithRandomStringOff)
        {
            if (jobConfigsWithRandomStringOff.Any())
                await Logger.SwallowAsync(async () =>
                {
                    var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Rating = GlobalConfig.Ratings });
                    var tasks = jobConfigsWithRandomStringOff.Select(jobConfig => PostRandomGif(jobConfig, giphyRandomResult.Data.Url)).ToList();
                    await Task.WhenAll(tasks);
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
            catch (HttpException httpException) when (GlobalConfig.Config.HttpExceptionsToWarn.Contains(httpException.Message))
            {
                Logger.Warn(httpException.Message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
