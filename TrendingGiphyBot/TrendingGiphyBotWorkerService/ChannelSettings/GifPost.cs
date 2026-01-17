namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class GifPost
{
    public Guid GifPostId { get; set; }
    public ulong ChannelId { get; set; }
    public required string GiphyDataId { get; set; }
    public required string GifUrl { get; set; }
    public bool IsTrending { get; set; }
    public required virtual ChannelSettingsModel ChannelSettingsModel { get; set; }
}