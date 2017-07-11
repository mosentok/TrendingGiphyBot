using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrendingGiphyBot.Containers
{
    class HelpContainer
    {
        [JsonProperty("Commands")]
        public IEnumerable<MethodContainer> Methods { get; set; }
        public HelpContainer(IEnumerable<MethodContainer> methods)
        {
            Methods = methods;
        }
    }
}
