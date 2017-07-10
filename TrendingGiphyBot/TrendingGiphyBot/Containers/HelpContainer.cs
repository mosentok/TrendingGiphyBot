﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrendingGiphyBot.Containers
{
    class HelpContainer
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public IEnumerable<string> Aliases { get; set; }
        [JsonProperty("Commands")]
        public IEnumerable<MethodContainer> Methods { get; set; }
        public HelpContainer(string name, string summary, IEnumerable<string> aliases, IEnumerable<MethodContainer> methods)
        {
            Name = name;
            Summary = summary;
            Aliases = aliases;
            Methods = methods;
        }
    }
}
