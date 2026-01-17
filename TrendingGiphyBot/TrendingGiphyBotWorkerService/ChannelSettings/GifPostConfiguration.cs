using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

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