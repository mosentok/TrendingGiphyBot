using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Database;

public interface ITrendingGiphyBotDbContext
{
    DbSet<ChannelSettingsModel> ChannelSettings { get; set; }
	Task SaveChangesAsync();
}
