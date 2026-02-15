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

        // Composite index for cleanup operations: find old posts by channel
        // Ordered DESC by CreatedUtc for efficient deletion of old records
        // This index also covers queries on ChannelId alone (leftmost column in composite index)
        builder.HasIndex(p => new { p.ChannelId, p.CreatedUtc })
            .IsDescending(false, true)
            .HasDatabaseName("IX_KlipyPosts_ChannelId_CreatedUtc");

        // Single column index for general age-based maintenance queries
        // Optimizes cleanup operations that find records older than a certain date
        builder.HasIndex(p => p.CreatedUtc)
            .HasDatabaseName("IX_KlipyPosts_CreatedUtc");
    }
}