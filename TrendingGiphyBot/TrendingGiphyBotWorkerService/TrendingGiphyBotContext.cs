using Microsoft.EntityFrameworkCore;

namespace TrendingGiphyBotWorkerService;

public class TrendingGiphyBotContext : DbContext, ITrendingGiphyBotContext
{
	public virtual DbSet<ChannelSettings> ChannelSettings { get; set; }

	public TrendingGiphyBotContext() { }

	public TrendingGiphyBotContext(DbContextOptions<TrendingGiphyBotContext> options) : base(options) { }

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

	public async Task SaveChangesAsync() => await base.SaveChangesAsync();
}
