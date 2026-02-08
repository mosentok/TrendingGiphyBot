using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class GiphyPostConfiguration : IEntityTypeConfiguration<GiphyPost>
{
    public void Configure(EntityTypeBuilder<GiphyPost> builder)
    {
        builder
            .HasOne(s => s.ChannelSettings)
            .WithMany(s => s.GiphyPosts)
            .HasForeignKey(s => s.ChannelId)
            .HasPrincipalKey(s => s.ChannelId);
    }
}
