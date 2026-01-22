using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Database;

public interface ITrendingGiphyBotDbContext
{
    DbSet<ChannelSettingsModel> ChannelSettings { get; set; }
    DbSet<GifPost> GifPosts { get; set; }
    DbSet<Interval> Intervals { get; set; }
	Task SaveChangesAsync(CancellationToken cancellation = default);
}
