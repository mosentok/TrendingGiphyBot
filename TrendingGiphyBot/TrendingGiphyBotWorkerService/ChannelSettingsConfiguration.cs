using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrendingGiphyBotWorkerService;

public class ChannelSettingsConfiguration : IEntityTypeConfiguration<ChannelSettings>
{
    public void Configure(EntityTypeBuilder<ChannelSettings> builder)
    {
        builder.HasKey(s => s.ChannelId);
    }
}