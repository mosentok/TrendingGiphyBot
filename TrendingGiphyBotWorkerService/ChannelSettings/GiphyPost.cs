namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class GiphyPost
{
    public long GiphyPostId { get; set; }
    public ulong ChannelId { get; set; }
    public required string GiphyDataId { get; set; }
    public DateTime CreatedUtc { get; set; }

    [LogPropertyIgnore]
    public virtual ChannelSettingsModel ChannelSettings { get; set; } = null!;
}
