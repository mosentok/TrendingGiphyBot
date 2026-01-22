namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class GifPost
{
    public long GifPostId { get; set; }
    public ulong ChannelId { get; set; }
    public required string GiphyDataId { get; set; }
    public virtual ChannelSettingsModel ChannelSettings { get; set; } = null!;
}