using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Database;

public interface ITrendingGiphyBotContext
{
    DbSet<ChannelSettingsModel> ChannelSettings { get; set; }
	Task SaveChangesAsync();
}
