namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class KlipyPost
{
    public long KlipyPostId { get; set; }
    public ulong ChannelId { get; set; }
    public required ulong KlipyDataId { get; set; }
    public DateTime CreatedUtc { get; set; }

    [LogPropertyIgnore]
    public virtual ChannelSettingsModel ChannelSettings { get; set; } = null!;
}
