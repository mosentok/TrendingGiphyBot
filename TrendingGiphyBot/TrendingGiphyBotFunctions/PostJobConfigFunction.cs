﻿using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc;
using TrendingGiphyBotFunctions.Extensions;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions
{
    public class PostJobConfigFunction
    {
        [FunctionName(nameof(PostJobConfigFunction))]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "jobconfigs/{channelid:decimal}")] HttpRequest req, decimal channelId, ILogger log)
        {
            var connectionString = Environment.GetEnvironmentVariable("TrendingGiphyBotConnectionString");
            var postJobConfigFunction = new PostJobConfigFunction(log, new TrendingGiphyBotContext(connectionString));
            var container = await req.Body.ReadToEndAsync<JobConfigContainer>();
            return await postJobConfigFunction.RunAsync(container, channelId);
        }
        readonly ILogger _Log;
        readonly ITrendingGiphyBotContext _Context;
        public PostJobConfigFunction(ILogger log, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task<IActionResult> RunAsync(JobConfigContainer container, decimal channelId)
        {
            _Log.LogInformation($"Channel {channelId} posting job config.");
            var result = await _Context.SetJobConfig(channelId, container);
            _Log.LogInformation($"Channel {channelId} posted job config.");
            return new OkObjectResult(result);
        }
    }
}
