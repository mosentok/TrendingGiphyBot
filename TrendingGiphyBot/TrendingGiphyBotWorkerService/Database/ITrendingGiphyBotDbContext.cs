using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Database;

public interface ITrendingGiphyBotDbContext
{
    DbSet<Interval> Intervals { get; set; }
    DbSet<ChannelSettingsModel> ChannelSettings { get; set; }
	Task SaveChangesAsync(CancellationToken cancellation = default);
}
