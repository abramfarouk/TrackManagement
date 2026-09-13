using Microsoft.EntityFrameworkCore;
using TrackManagement.Domain.Entities;
using TrackManagement.Domain.Enums;
using TrackManagement.Persistence.Context;

namespace TrackManagement.Persistence.Seed
{
    public static class DbSeeder
    {
    
        public static async Task SeedAsync(TrackManagementDbContext db)
        {
            await db.Database.MigrateAsync();

            if (await db.Artists.AnyAsync())
            {
                return; 
            }

            var artists = new List<Artist>
        {
            new() { Name = "Nadia Karam", Email = "nadia.karam@example.com", Country = "Egypt" },
            new() { Name = "Marcus Idowu", Email = "marcus.idowu@example.com", Country = "Nigeria" },
            new() { Name = "Elena Ruiz", Email = "elena.ruiz@example.com", Country = "Spain" },
            new() { Name = "Kenji Watanabe", Email = "kenji.watanabe@example.com", Country = "Japan" }
        };
            await db.Artists.AddRangeAsync(artists);

            var dsps = new List<Dsp>
        {
            new() { Name = "Spotify" },
            new() { Name = "Apple Music" },
            new() { Name = "YouTube Music" }
        };
            await db.Dsps.AddRangeAsync(dsps);

            await db.SaveChangesAsync(); 

            var tracks = new List<Track>
        {
            new()
            {
                Title = "Nile Nights",
                ArtistId = artists[0].Id,
                Isrc = "EGX240100001",
                ReleaseDate = new DateTime(2024, 3, 10, 0, 0, 0, DateTimeKind.Utc),
                Genre = "Pop",
                Status = TrackStatus.Distributed
            },
            new()
            {
                Title = "Desert Bloom",
                ArtistId = artists[0].Id,
                Isrc = "EGX240100002",
                ReleaseDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc),
                Genre = "Chillout",
                Status = TrackStatus.Submitted
            },
            new()
            {
                Title = "Lagos Groove",
                ArtistId = artists[1].Id,
                Isrc = "NGX240200001",
                ReleaseDate = new DateTime(2024, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                Genre = "Afrobeat",
                Status = TrackStatus.Distributed
            },
            new()
            {
                Title = "Danfo Anthem",
                ArtistId = artists[1].Id,
                Isrc = "NGX240200002",
                ReleaseDate = new DateTime(2024, 8, 15, 0, 0, 0, DateTimeKind.Utc),
                Genre = "Afrobeat",
                Status = TrackStatus.Draft
            },
            new()
            {
                Title = "Flamenco Dreams",
                ArtistId = artists[2].Id,
                Isrc = "ESX240300001",
                ReleaseDate = new DateTime(2023, 11, 5, 0, 0, 0, DateTimeKind.Utc),
                Genre = "Flamenco",
                Status = TrackStatus.Distributed
            },
            new()
            {
                Title = "Madrid Midnight",
                ArtistId = artists[2].Id,
                Isrc = "ESX240300002",
                ReleaseDate = new DateTime(2024, 5, 22, 0, 0, 0, DateTimeKind.Utc),
                Genre = "Electronic",
                Status = TrackStatus.Submitted
            },
            new()
            {
                Title = "Shibuya Rain",
                ArtistId = artists[3].Id,
                Isrc = "JPX240400001",
                ReleaseDate = new DateTime(2024, 2, 14, 0, 0, 0, DateTimeKind.Utc),
                Genre = "J-Pop",
                Status = TrackStatus.Draft
            },
            new()
            {
                Title = "Cherry Static",
                ArtistId = artists[3].Id,
                Isrc = "JPX240400002",
                ReleaseDate = new DateTime(2024, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                Genre = "Electronic",
                Status = TrackStatus.Distributed
            },
            new()
            {
                Title = "Quiet Harbor",
                ArtistId = artists[3].Id,
                Isrc = "JPX240400003",
                ReleaseDate = new DateTime(2024, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                Genre = "Ambient",
                Status = TrackStatus.Submitted
            }
        };
            await db.Tracks.AddRangeAsync(tracks);
            await db.SaveChangesAsync();

            var spotify = dsps[0];
            var apple = dsps[1];
            var youtube = dsps[2];
            var now = DateTime.UtcNow;

            var distributions = new List<TrackDistribution>
        {
            new() { TrackId = tracks[0].Id, DspId = spotify.Id, SubmittedAt = now.AddDays(-30), Status = DistributionStatus.Live },
            new() { TrackId = tracks[0].Id, DspId = apple.Id, SubmittedAt = now.AddDays(-30), Status = DistributionStatus.Live },
            new() { TrackId = tracks[1].Id, DspId = spotify.Id, SubmittedAt = now.AddDays(-2), Status = DistributionStatus.Pending },
            new() { TrackId = tracks[2].Id, DspId = spotify.Id, SubmittedAt = now.AddDays(-60), Status = DistributionStatus.Live },
            new() { TrackId = tracks[2].Id, DspId = youtube.Id, SubmittedAt = now.AddDays(-60), Status = DistributionStatus.Live },
            new() { TrackId = tracks[4].Id, DspId = apple.Id, SubmittedAt = now.AddDays(-90), Status = DistributionStatus.Live },
            new() { TrackId = tracks[4].Id, DspId = spotify.Id, SubmittedAt = now.AddDays(-90), Status = DistributionStatus.Rejected },
            new() { TrackId = tracks[5].Id, DspId = youtube.Id, SubmittedAt = now.AddDays(-5), Status = DistributionStatus.Pending },
            new() { TrackId = tracks[7].Id, DspId = spotify.Id, SubmittedAt = now.AddDays(-15), Status = DistributionStatus.Live },
            new() { TrackId = tracks[7].Id, DspId = apple.Id, SubmittedAt = now.AddDays(-15), Status = DistributionStatus.Live },
            new() { TrackId = tracks[7].Id, DspId = youtube.Id, SubmittedAt = now.AddDays(-15), Status = DistributionStatus.Live },
            new() { TrackId = tracks[8].Id, DspId = apple.Id, SubmittedAt = now.AddDays(-1), Status = DistributionStatus.Pending }
        };
            await db.TrackDistributions.AddRangeAsync(distributions);

            await db.SaveChangesAsync();
        }
    }

}
