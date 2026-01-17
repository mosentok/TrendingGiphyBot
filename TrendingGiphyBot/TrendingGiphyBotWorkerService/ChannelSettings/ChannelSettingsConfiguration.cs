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
