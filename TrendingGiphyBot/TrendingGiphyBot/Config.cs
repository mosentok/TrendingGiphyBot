namespace TrendingGiphyBot
{
    class Config
    {
        public string DiscordToken { get; set; }
        public string GiphyToken { get; set; }
        public int Hour { get; set; }
        public int Minutes { get; set; }
        public int RunEveryXSeconds { get; set; } = 5;
    }
}
