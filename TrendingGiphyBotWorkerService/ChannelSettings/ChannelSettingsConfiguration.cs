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

		var navigationBuilder = builder.OwnsOne(s => s.PostingHours);

		navigationBuilder
			.Property(r => r.From)
			.HasColumnName("PostingHoursFrom");

		navigationBuilder
			.Property(r => r.To)
			.HasColumnName("PostingHoursTo");

		navigationBuilder
			.Property(r => r.UtcOffset)
			.HasColumnName("UtcOffset");
	}}