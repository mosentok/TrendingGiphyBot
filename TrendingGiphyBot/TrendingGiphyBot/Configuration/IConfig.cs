namespace TrendingGiphyBot
{
    interface IConfig
    {
        string ConnectionString { get; set; }
        string DiscordToken { get; set; }
        string GiphyToken { get; set; }
        int MinimumMinutes { get; set; }
        string WordnikToken { get; set; }
    }
}