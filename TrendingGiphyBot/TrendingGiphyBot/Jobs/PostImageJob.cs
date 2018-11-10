using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using TrendingGiphyBot.Dals;
using NLog;
using System.Collections.Generic;
using System.Linq;
using Discord.Net;
using GiphyDotNet.Model.Parameters;
using Newtonsoft.Json;
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
            var currentValidMinutes = DetermineCurrentValidMinutes();
            if (currentValidMinutes.Any())
            {
                var successes = new List<UrlHistory>();
                var jobConfigs = await GlobalConfig.JobConfigDal.Get(currentValidMinutes);
                var jobConfigsNotInQuietHours = jobConfigs.Where(s => !s.IsInQuietHours()).ToList();
                var jobConfigsJustPostedTo = await PostChannelsNotInQuietHours(jobConfigsNotInQuietHours, successes);
                var remainingJobConfigs = jobConfigsNotInQuietHours.Except(jobConfigsJustPostedTo).Where(s => s.RandomIsOn).ToList();
                var jobConfigsWithRandomStringOn = remainingJobConfigs.Where(s => !string.IsNullOrEmpty(s.RandomSearchString)).ToList();
                var jobConfigsWithRandomStringOff = remainingJobConfigs.Except(jobConfigsWithRandomStringOn).ToList();
                await PostChannelsWithRandomStringOn(jobConfigsWithRandomStringOn, successes);
                await PostChannelsWithRandomStringOff(jobConfigsWithRandomStringOff, successes);
                await GlobalConfig.UrlHistoryDal.Insert(successes);
            }
        }
        List<int> DetermineCurrentValidMinutes()
        {
            var now = DateTime.Now;
            var totalMinutes = now.Hour * 60 + now.Minute;
            if (totalMinutes == 0)
                totalMinutes = 24 * 60;
            return GlobalConfig.AllValidMinutes.Where(s => totalMinutes % s == 0).ToList();
        }
        async Task<List<JobConfig>> PostChannelsNotInQuietHours(List<JobConfig> jobConfigsNotInQuietHours, List<UrlHistory> successes)
        {
            var channelsPostedTo = new List<JobConfig>();
            foreach (var jobConfig in jobConfigsNotInQuietHours)
                if (DiscordClient.GetChannel(jobConfig.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                    await PostFirstNewGif(channelsPostedTo, jobConfig, socketTextChannel, successes);
            return channelsPostedTo;
        }
        async Task PostFirstNewGif(List<JobConfig> channelsPostedTo, JobConfig jobConfig, SocketTextChannel socketTextChannel, List<UrlHistory> successes)
        {
            var url = await GlobalConfig.UrlHistoryDal.GetLastestUrlNotPosted(jobConfig.ChannelId);
            if (!string.IsNullOrEmpty(url))
            {
                await PostGif(jobConfig.ChannelId, url, $"*Trending!* {url}", socketTextChannel, successes);
                channelsPostedTo.Add(jobConfig);
            }
        }
        async Task PostChannelsWithRandomStringOn(List<JobConfig> jobConfigsWithRandomStringOn, List<UrlHistory> successes)
        {
            foreach (var jobConfig in jobConfigsWithRandomStringOn)
                await Logger.SwallowAsync(async () =>
                {
                    try
                    {
                        var randomParameter = new RandomParameter { Rating = GlobalConfig.Ratings, Tag = jobConfig.RandomSearchString };
                        var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(randomParameter);
                        await PostRandomGif(jobConfig, giphyRandomResult.Data.Url, successes);
                    }
                    catch (JsonSerializationException jsEx)
                    {
                        Logger.Error(jsEx);
                        await Logger.SwallowAsync(GlobalConfig.JobConfigDal.BlankRandomConfig(jobConfig.ChannelId));
                    }
                });
        }
        async Task PostChannelsWithRandomStringOff(List<JobConfig> jobConfigsWithRandomStringOff, List<UrlHistory> successes)
        {
            if (jobConfigsWithRandomStringOff.Any())
                await Logger.SwallowAsync(async () =>
                {
                    var giphyRandomResult = await GlobalConfig.GiphyClient.RandomGif(new RandomParameter { Rating = GlobalConfig.Ratings });
                    foreach (var jobConfig in jobConfigsWithRandomStringOff)
                        await PostRandomGif(jobConfig, giphyRandomResult.Data.Url, successes);
                });
        }
        async Task PostRandomGif(JobConfig jobConfig, string url, List<UrlHistory> successes)
        {
            if (!await GlobalConfig.UrlHistoryDal.Any(jobConfig.ChannelId, url)
                && DiscordClient.GetChannel(jobConfig.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                await PostGif(jobConfig.ChannelId, url, url, socketTextChannel, successes);
        }
        async Task PostGif(decimal channelId, string url, string message, SocketTextChannel socketTextChannel, List<UrlHistory> successes)
        {
            try
            {
                await socketTextChannel.SendMessageAsync(message);
                var history = new UrlHistory { ChannelId = channelId, Url = url };
                successes.Add(history);
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
