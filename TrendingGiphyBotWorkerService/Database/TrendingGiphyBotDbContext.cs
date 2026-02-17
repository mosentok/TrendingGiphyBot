using Microsoft.EntityFrameworkCore;
using TrendingGiphyBotWorkerService.ChannelSettings;

namespace TrendingGiphyBotWorkerService.Database;

public class TrendingGiphyBotDbContext : DbContext, ITrendingGiphyBotDbContext
{
	public virtual DbSet<ChannelSettingsModel> ChannelSettings { get; set; }
	public virtual DbSet<GiphyPost> GiphyPosts { get; set; }
    public virtual DbSet<KlipyPost> KlipyPosts { get; set; }

    public TrendingGiphyBotDbContext() { }

	public TrendingGiphyBotDbContext(DbContextOptions<TrendingGiphyBotDbContext> options) : base(options) { }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		if (optionsBuilder.IsConfigured)
			return;

		var databasePath = Path.Combine(AppContext.BaseDirectory, "app.db");

		optionsBuilder
			.EnableSensitiveDataLogging()
			.UseSqlite($"Data Source={databasePath}");
	}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		var type = GetType();

		modelBuilder.ApplyConfigurationsFromAssembly(type.Assembly);
    }

	public new Task SaveChangesAsync(CancellationToken cancellation = default) => base.SaveChangesAsync(cancellation);
}
