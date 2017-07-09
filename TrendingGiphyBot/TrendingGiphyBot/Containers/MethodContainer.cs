using System.Collections.Generic;

namespace TrendingGiphyBot.Containers
{
    class MethodContainer
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public IEnumerable<ParameterContainer> Parameters { get; set; }
        public MethodContainer(string name, string summary, IEnumerable<ParameterContainer> parameters)
        {
            Name = name;
            Summary = summary;
            Parameters = parameters;
        }
    }
}
