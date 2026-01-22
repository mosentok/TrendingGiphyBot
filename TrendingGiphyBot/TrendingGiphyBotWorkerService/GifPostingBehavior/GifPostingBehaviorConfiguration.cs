using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrendingGiphyBotWorkerService.GifPostingBehavior;

public class GifPostingBehaviorConfiguration : IEntityTypeConfiguration<GifPostingBehaviorModel>
{
    public void Configure(EntityTypeBuilder<GifPostingBehaviorModel> builder)
    {
        builder.Property(s => s.GifPostingBehaviorId).ValueGeneratedNever();
        builder.HasKey(s => s.GifPostingBehaviorId);

        builder
            .HasMany(s => s.ChannelSettings)
            .WithOne(s => s.GifPostingBehavior)
            .HasPrincipalKey(s => s.GifPostingBehaviorId)
            .HasForeignKey(s => s.GifPostingBehaviorId);
    }
}
