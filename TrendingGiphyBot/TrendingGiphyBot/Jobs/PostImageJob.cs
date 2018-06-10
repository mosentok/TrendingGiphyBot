using System;
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
using TrendingGiphyBot.Enums;
using TrendingGiphyBot.Extensions;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        internal PostImageJob(IGlobalConfig globalConfig) : base(globalConfig, LogManager.GetCurrentClassLogger(), new SubJobConfig(5, Time.Minutes)) { }
        protected override async Task Run()
        {
            await Logger.SwallowAsync(async () =>
            {
                if (GlobalConfig.LatestUrls.Any())
                {
                    var currentValidMinutes = DetermineCurrentValidMinutes();
                    if (currentValidMinutes.Any())
                    {
                        var jobConfigs = await GlobalConfig.JobConfigDal.Get(currentValidMinutes);
                        var jobConfigsNotInQuietHours = jobConfigs.Where(s => !s.IsInQuietHours()).ToList();
                        var jobConfigsJustPostedTo = await PostChannelsNotInQuietHours(jobConfigsNotInQuietHours);
                        var remainingJobConfigs = jobConfigsNotInQuietHours.Except(jobConfigsJustPostedTo).Where(s => s.RandomIsOn).ToList();
                        var jobConfigsWithRandomStringOn = remainingJobConfigs.Where(s => !string.IsNullOrEmpty(s.RandomSearchString)).ToList();
                        var jobConfigsWithRandomStringOff = remainingJobConfigs.Except(jobConfigsWithRandomStringOn).ToList();
                        await PostChannelsWithRandomStringOn(jobConfigsWithRandomStringOn);
                        await PostChannelsWithRandomStringOff(jobConfigsWithRandomStringOff);
                    }
                }
            });
        }
        List<int> DetermineCurrentValidMinutes()
        {
            var now = DateTime.Now;
            var totalMinutes = now.Hour * 60 + now.Minute;
            if (totalMinutes == 0)
                totalMinutes = 24 * 60;
            return GlobalConfig.AllValidMinutes.Where(s => totalMinutes % s == 0).ToList();
        }
        async Task<ConcurrentBag<JobConfig>> PostChannelsNotInQuietHours(List<JobConfig> jobConfigsNotInQuietHours)
        {
            var channelsPostedTo = new ConcurrentBag<JobConfig>();
            foreach (var jobConfig in jobConfigsNotInQuietHours)
                if (DiscordClient.GetChannel(jobConfig.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                    await PostFirstNewGif(channelsPostedTo, jobConfig, socketTextChannel);
            return channelsPostedTo;
        }
        async Task PostFirstNewGif(ConcurrentBag<JobConfig> channelsPostedTo, JobConfig jobConfig, SocketTextChannel socketTextChannel)
        {
            var url = await GlobalConfig.LatestUrls
                .OrderByDescending(s => s.Stamp)
                .Select(s => s.Url)
                .FirstOrDefaultAsync(async s => !await GlobalConfig.UrlHistoryDal.Any(jobConfig.ChannelId, s));
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
                    var randomParameter = new RandomParameter { Rating = GlobalConfig.Ratings, Tag = jobConfig.RandomSearchString };
                    var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(randomParameter);
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
            catch (HttpException httpException) when (GlobalConfig.Config.HttpExceptionsToWarn.Contains(httpException.Message))
            {
                Logger.Warn(httpException.Message);
                await Logger.SwallowAsync(GlobalConfig.JobConfigDal.Remove(channelId));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
