using System.Collections.Generic;

namespace TrendingGiphyBot.Containers
{
    class MethodContainer
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public IEnumerable<FieldContainer> Fields { get; set; }
        public MethodContainer(string name, string summary, IEnumerable<FieldContainer> fields)
        {
            Name = name;
            Summary = summary;
            Fields = fields;
        }
    }
}
