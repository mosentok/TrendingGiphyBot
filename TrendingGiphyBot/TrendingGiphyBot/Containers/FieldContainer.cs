namespace TrendingGiphyBot.Containers
{
    class FieldContainer
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public FieldContainer(string name, string summary)
        {
            Name = name;
            Summary = summary;
        }
    }
}
