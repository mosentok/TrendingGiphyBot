using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Database;

public interface ITrendingGiphyBotDbContext
{
    DbSet<ChannelSettingsModel> ChannelSettings { get; set; }
    DbSet<GiphyPost> GiphyPosts { get; set; }
    DbSet<KlipyPost> KlipyPosts { get; set; }
	Task SaveChangesAsync(CancellationToken cancellation = default);
}
