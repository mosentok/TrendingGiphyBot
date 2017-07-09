namespace TrendingGiphyBot.Containers
{
    class ParameterContainer
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public ParameterContainer(string name, string summary)
        {
            Name = name;
            Summary = summary;
        }
    }
}
