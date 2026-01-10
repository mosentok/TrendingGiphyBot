using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class ChannelSettingsConfiguration : IEntityTypeConfiguration<ChannelSettingsModel>
{
	public void Configure(EntityTypeBuilder<ChannelSettingsModel> builder)
	{
		builder
			.ToTable("ChannelSettings")
			.HasKey(s => s.ChannelId);
	}
}

public class GifPostConfiguration : IEntityTypeConfiguration<GifPost>
{
	public void Configure(EntityTypeBuilder<GifPost> builder)
	{
		builder
			.HasOne(s => s.ChannelSettingsModel)
			.WithMany(s => s.GifPosts)
			.HasForeignKey(s => s.ChannelId)
			.HasPrincipalKey(s => s.ChannelId);
	}
}