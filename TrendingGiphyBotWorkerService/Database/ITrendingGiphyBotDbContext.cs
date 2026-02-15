using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.GifPostingBehavior;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Database;

public interface ITrendingGiphyBotDbContext
{
    DbSet<ChannelSettingsModel> ChannelSettings { get; set; }
    DbSet<GifPostingBehaviorModel> GifPostingBehaviors { get; set; }
    DbSet<GiphyPost> GiphyPosts { get; set; }
    DbSet<KlipyPost> KlipyPosts { get; set; }
    DbSet<Interval> Intervals { get; set; }
	Task SaveChangesAsync(CancellationToken cancellation = default);
}
