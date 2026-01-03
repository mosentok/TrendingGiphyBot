using Microsoft.EntityFrameworkCore;

namespace TrendingGiphyBotWorkerService;

public interface ITrendingGiphyBotContext
{
    DbSet<ChannelSettings> ChannelSettings { get; set; }
	Task SaveChangesAsync();
}
