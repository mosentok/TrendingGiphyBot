using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using TrendingGiphyBot.Dals;
using NLog;
using System.Collections.Generic;
using System.Linq;
using Discord.Net;
using Newtonsoft.Json;
using TrendingGiphyBot.Configuration;
using TrendingGiphyBot.Extensions;
using System.Net.Http;
using TrendingGiphyBot.Containers;
using TrendingGiphyBot.Helpers;

namespace TrendingGiphyBot.Jobs
{
    class PostImageJob : Job
    {
        static readonly HttpClient _HttpClient = new HttpClient();
        readonly IFunctionHelper _FunctionHelper;
        internal PostImageJob(IGlobalConfig globalConfig, IFunctionHelper functionHelper, SubJobConfig subJobConfig) : base(globalConfig, LogManager.GetCurrentClassLogger(), subJobConfig)
        {
            _FunctionHelper = functionHelper;
        }
        protected override async Task Run()
        {
            var currentValidMinutes = DetermineCurrentValidMinutes();
            if (currentValidMinutes.Any())
            {
                var successes = new List<UrlHistory>();
                using (var entities = GlobalConfig.EntitiesFactory.GetNewTrendingGiphyBotEntities())
                {
                    var jobConfigsToRun = await entities.GetJobConfigsToRun(currentValidMinutes);
                    var nowHour = DateTime.Now.Hour - GlobalConfig.Config.HourOffset;
                    var jobConfigsNotInQuietHours = jobConfigsToRun.Where(s => !s.IsInQuietHours(nowHour)).ToList();
                    var jobConfigsJustPostedTo = await PostChannelsNotInQuietHours(entities, jobConfigsNotInQuietHours, successes);
                    var remainingJobConfigs = jobConfigsNotInQuietHours.Except(jobConfigsJustPostedTo).Where(s => s.RandomIsOn).ToList();
                    var jobConfigsWithRandomStringOn = remainingJobConfigs.Where(s => !string.IsNullOrEmpty(s.RandomSearchString)).ToList();
                    var jobConfigsWithRandomStringOff = remainingJobConfigs.Except(jobConfigsWithRandomStringOn).ToList();
                    await PostChannelsWithRandomStringOn(entities, jobConfigsWithRandomStringOn, successes);
                    await PostChannelsWithRandomStringOff(entities, jobConfigsWithRandomStringOff, successes);
                    await entities.InsertUrlHistories(successes);
                }
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
        async Task<List<JobConfig>> PostChannelsNotInQuietHours(TrendingGiphyBotEntities entities, List<JobConfig> jobConfigsNotInQuietHours, List<UrlHistory> successes)
        {
            var channelsPostedTo = new List<JobConfig>();
            foreach (var jobConfig in jobConfigsNotInQuietHours)
                if (DiscordClient.GetChannel(jobConfig.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                    await PostFirstNewGif(entities, channelsPostedTo, jobConfig, socketTextChannel, successes);
            return channelsPostedTo;
        }
        async Task PostFirstNewGif(TrendingGiphyBotEntities entities, List<JobConfig> channelsPostedTo, JobConfig jobConfig, SocketTextChannel socketTextChannel, List<UrlHistory> successes)
        {
            var url = await entities.GetLastestUrlNotPosted(jobConfig.ChannelId);
            if (!string.IsNullOrEmpty(url))
            {
                await PostGif(entities, jobConfig.ChannelId, url, $"*Trending!* {url}", socketTextChannel, successes);
                channelsPostedTo.Add(jobConfig);
            }
        }
        async Task PostChannelsWithRandomStringOn(TrendingGiphyBotEntities entities, List<JobConfig> jobConfigsWithRandomStringOn, List<UrlHistory> successes)
        {
            foreach (var jobConfig in jobConfigsWithRandomStringOn)
                await Logger.SwallowAsync(async () =>
                {
                    try
                    {
                        var endpointWithTag = $"{GlobalConfig.GiphyRandomEndpoint}&tag={jobConfig.RandomSearchString}";
                        var giphyRandomResult = await GetRandomResult(endpointWithTag);
                        await PostRandomGif(entities, jobConfig, giphyRandomResult.Data.Url, successes);
                    }
                    catch (JsonSerializationException jsEx)
                    {
                        Logger.Error(jsEx, "Note that the exception handler used to remove the random search string, but no longer does. The hope is that now that the GIF object structure is simpler, whatever used to cause the exception is gone.");
                    }
                });
        }
        static async Task<GiphyRandomResponse> GetRandomResult(string requestUri)
        {
            var response = await _HttpClient.GetAsync(requestUri);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GiphyRandomResponse>(content);
        }
        async Task PostChannelsWithRandomStringOff(TrendingGiphyBotEntities entities, List<JobConfig> jobConfigsWithRandomStringOff, List<UrlHistory> successes)
        {
            if (jobConfigsWithRandomStringOff.Any())
                await Logger.SwallowAsync(async () =>
                {
                    var giphyRandomResult = await GetRandomResult(GlobalConfig.GiphyRandomEndpoint);
                    foreach (var jobConfig in jobConfigsWithRandomStringOff)
                        await PostRandomGif(entities, jobConfig, giphyRandomResult.Data.Url, successes);
                });
        }
        async Task PostRandomGif(TrendingGiphyBotEntities entities, JobConfig jobConfig, string url, List<UrlHistory> successes)
        {
            if (!await entities.AnyUrlHistories(jobConfig.ChannelId, url)
                && DiscordClient.GetChannel(jobConfig.ChannelId.ToULong()) is SocketTextChannel socketTextChannel)
                await PostGif(entities, jobConfig.ChannelId, url, url, socketTextChannel, successes);
        }
        async Task PostGif(TrendingGiphyBotEntities entities, decimal channelId, string url, string message, SocketTextChannel socketTextChannel, List<UrlHistory> successes)
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
                await Logger.SwallowAsync(_FunctionHelper.DeleteJobConfigAsync(channelId));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
