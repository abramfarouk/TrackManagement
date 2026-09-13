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

    public class TrackDistributionConfiguration : IEntityTypeConfiguration<TrackDistribution>
    {
        public void Configure(EntityTypeBuilder<TrackDistribution> builder)
        {
            builder.ToTable("TrackDistributions");

            builder.HasKey(td => td.Id);

            builder.Property(td => td.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasDefaultValue(DistributionStatus.Pending);

            builder.Property(td => td.SubmittedAt)
                .IsRequired();

            builder.HasOne(td => td.Dsp)
                .WithMany(d => d.TrackDistributions)
                .HasForeignKey(td => td.DspId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(td => new { td.TrackId, td.DspId }).IsUnique();
        }
    }
}
