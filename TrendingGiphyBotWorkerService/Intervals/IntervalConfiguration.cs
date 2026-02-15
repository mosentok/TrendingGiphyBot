using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrendingGiphyBotWorkerService.Intervals;

public class IntervalConfiguration : IEntityTypeConfiguration<Interval>
{
    public void Configure(EntityTypeBuilder<Interval> builder)
    {
        builder.Property(s => s.IntervalId).ValueGeneratedNever();

        builder.HasMany(s => s.ChannelSettings)
            .WithOne(s => s.Interval)
            .HasPrincipalKey(s => s.IntervalId)
            .HasForeignKey(s => s.IntervalId);
    }
}
