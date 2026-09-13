using TrackManagement.Domain.Common;

namespace TrackManagement.Domain.Entities
{
    public class Track : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public int ArtistId { get; set; }

        public Artist? Artist { get; set; }

        public string Isrc { get; set; } = string.Empty;

        public DateTime ReleaseDate { get; set; }

        public string Genre { get; set; } = string.Empty;

        public TrackStatus Status { get; set; } = TrackStatus.Draft;

        public ICollection<TrackDistribution> TrackDistributions { get; set; } = new List<TrackDistribution>();
    }

}