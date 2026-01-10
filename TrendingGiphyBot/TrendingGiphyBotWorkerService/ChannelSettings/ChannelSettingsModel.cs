namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class ChannelSettingsModel
{
    public ulong ChannelId { get; set; }
    public string? HowOften { get; set; }
    public string? GifPostingBehavior { get; set; }
    public string? GifKeyword { get; set; }
    public string? PostingHoursFrom { get; set; }
    public string? PostingHoursTo { get; set; }
    public string? TimeZone { get; set; }
    public virtual ICollection<GifPost>? GifPosts { get; set; }
}
public class GifPost
{
    public Guid GifPostId { get; set; }
    public ulong ChannelId { get; set; }
    public required string GiphyDataId { get; set; }
    public required string GifUrl { get; set; }
    public bool IsTrending { get; set; }
    public required virtual ChannelSettingsModel ChannelSettingsModel { get; set; }
}