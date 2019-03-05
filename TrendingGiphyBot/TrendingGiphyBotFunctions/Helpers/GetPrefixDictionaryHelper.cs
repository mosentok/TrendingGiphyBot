using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrendingGiphyBotFunctions.Wrappers;
using TrendingGiphyBotModel;

namespace TrendingGiphyBotFunctions.Helpers
{
    public class GetPrefixDictionaryHelper
    {
        readonly ILoggerWrapper _Log;
        readonly ITrendingGiphyBotContext _Context;
        public GetPrefixDictionaryHelper(ILoggerWrapper log, ITrendingGiphyBotContext context)
        {
            _Log = log;
            _Context = context;
        }
        public async Task<Dictionary<decimal, string>> RunAsync()
        {
            _Log.LogInformation("Getting prefix dictionary.");
            var prefixDictionary = await _Context.GetPrefixDictionary();
            _Log.LogInformation($"Got {prefixDictionary.Count} prefixes.");
            return prefixDictionary;
        }
    }
}
