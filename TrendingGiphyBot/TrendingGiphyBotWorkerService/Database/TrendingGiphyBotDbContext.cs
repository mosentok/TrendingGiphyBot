using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;
using TrendingGiphyBotWorkerService.Intervals;

namespace TrendingGiphyBotWorkerService.Database;

public class TrendingGiphyBotDbContext : DbContext, ITrendingGiphyBotDbContext
{
	public virtual DbSet<ChannelSettingsModel> ChannelSettings { get; set; }
	public virtual DbSet<Interval> Intervals { get; set; }

	public TrendingGiphyBotDbContext() { }

	public TrendingGiphyBotDbContext(DbContextOptions<TrendingGiphyBotDbContext> options) : base(options) { }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (optionsBuilder.IsConfigured)
			return;

		var currentDirectory = Directory.GetCurrentDirectory();
		var databasePath = Path.Combine(currentDirectory, "app.db");

		optionsBuilder.UseSqlite($"Data Source={databasePath}");
	}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		var type = GetType();

		modelBuilder.ApplyConfigurationsFromAssembly(type.Assembly);
    }

	public new Task SaveChangesAsync(CancellationToken cancellation = default) => base.SaveChangesAsync(cancellation);
}
