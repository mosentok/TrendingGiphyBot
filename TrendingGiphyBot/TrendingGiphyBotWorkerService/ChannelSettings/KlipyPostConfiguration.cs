using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrendingGiphyBotWorkerService.ChannelSettings;

public class KlipyPostConfiguration : IEntityTypeConfiguration<KlipyPost>
{
    public void Configure(EntityTypeBuilder<KlipyPost> builder)
    {
        builder
            .HasOne(s => s.ChannelSettings)
            .WithMany(s => s.KlipyPosts)
            .HasForeignKey(s => s.ChannelId)
            .HasPrincipalKey(s => s.ChannelId);
    }
}