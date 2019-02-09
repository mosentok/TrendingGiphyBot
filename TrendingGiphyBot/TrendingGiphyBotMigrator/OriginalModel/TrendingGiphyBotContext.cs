using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TrendingGiphyBotMigrator.OriginalModel
{
    public partial class TrendingGiphyBotContext : DbContext
    {
        public TrendingGiphyBotContext()
        {
        }

        public TrendingGiphyBotContext(DbContextOptions<TrendingGiphyBotContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChannelConfig> ChannelConfig { get; set; }
        public virtual DbSet<JobConfig> JobConfig { get; set; }
        public virtual DbSet<Time> Time { get; set; }
        public virtual DbSet<UrlCache> UrlCache { get; set; }
        public virtual DbSet<UrlHistory> UrlHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity<ChannelConfig>(entity =>
            {
                entity.HasKey(e => e.ChannelId)
                    .HasName("PK__ChannelC__38C3E814C4B39CD1");

                entity.Property(e => e.ChannelId).HasColumnType("numeric(20, 0)");

                entity.Property(e => e.Prefix)
                    .IsRequired()
                    .HasMaxLength(4)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<JobConfig>(entity =>
            {
                entity.HasKey(e => e.ChannelId)
                    .HasName("PK__JobConfi__38C3E814A891663C");

                entity.Property(e => e.ChannelId).HasColumnType("numeric(20, 0)");

                entity.Property(e => e.RandomSearchString)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Time)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.HasOne(d => d.TimeNavigation)
                    .WithMany(p => p.JobConfig)
                    .HasForeignKey(d => d.Time)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_JobConfig_Time");
            });

            modelBuilder.Entity<Time>(entity =>
            {
                entity.HasKey(e => e.Value)
                    .HasName("PK__Time__07D9BBC369D4730F");

                entity.Property(e => e.Value)
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<UrlCache>(entity =>
            {
                entity.HasKey(e => e.Url)
                    .HasName("PK__UrlCache__C5B2143039EE6881");

                entity.Property(e => e.Url)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Stamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<UrlHistory>(entity =>
            {
                entity.HasKey(e => new { e.ChannelId, e.Url });

                entity.HasIndex(e => e.Stamp)
                    .HasName("nci_wi_UrlHistory_7886CC02C4D996621453061A88A8026B");

                entity.Property(e => e.ChannelId).HasColumnType("numeric(20, 0)");

                entity.Property(e => e.Url)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Stamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");
            });
        }
    }
}
