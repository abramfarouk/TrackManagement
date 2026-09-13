using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Domain.Entities;
using TrackManagement.Domain.Enums;

namespace TrackManagement.Persistence.Configurations
{
    public class TrackConfiguration : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            builder.ToTable("Tracks");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(t => t.Isrc)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(t => t.Isrc).IsUnique();

            builder.Property(t => t.Genre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(TrackStatus.Draft);

            builder.HasOne(t => t.Artist)
                .WithMany(a => a.Tracks)
                .HasForeignKey(t => t.ArtistId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.TrackDistributions)
                .WithOne(td => td.Track)
                .HasForeignKey(td => td.TrackId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
