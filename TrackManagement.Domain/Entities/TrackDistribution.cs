using TrackManagement.Domain.Common;
using TrackManagement.Domain.Enums;

namespace TrackManagement.Domain.Entities
{
    public class TrackDistribution : BaseEntity
    {
        public int TrackId { get; set; }

        public Track? Track { get; set; }

        public int DspId { get; set; }

        public Dsp? Dsp { get; set; }

        public DateTime SubmittedAt { get; set; }

        public DistributionStatus Status { get; set; } = DistributionStatus.Pending;
    }
}