using Microsoft.EntityFrameworkCore;
using TrackManagement.Application.DTOs;
using TrackManagement.Application.Interfaces;
using TrackManagement.Application.Interfaces.Repositories;
using TrackManagement.Domain.Entities;
using TrackManagement.Domain.Enums;
using TrackManagement.Persistence.Context;

namespace TrackManagement.Persistence.Repositories;

public class TrackRepository : ITrackRepository
{
    private readonly TrackManagementDbContext _db;

    public TrackRepository(TrackManagementDbContext db)
    {
        _db = db;
    }

    public Task<Track?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Tracks.FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<Track?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default) =>
        _db.Tracks
            .Include(t => t.Artist)
            .Include(t => t.TrackDistributions)
                .ThenInclude(td => td.Dsp)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<bool> IsrcExistsAsync(string isrc, CancellationToken ct = default) =>
        _db.Tracks.AnyAsync(t => t.Isrc.ToLower() == isrc.ToLower(), ct);

    public async Task<List<Track>> GetFilteredAsync(TrackFilterDto filter, CancellationToken ct = default)
    {
        var query = _db.Tracks.Include(t => t.Artist).AsQueryable();

        if (filter.ArtistId.HasValue)
        {
            query = query.Where(t => t.ArtistId == filter.ArtistId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Genre))
        {
            query = query.Where(t => t.Genre.ToLower() == filter.Genre.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(filter.Status) &&
            Enum.TryParse<TrackStatus>(filter.Status, true, out var status))
        {
            query = query.Where(t => t.Status == status);
        }

        return await query.OrderByDescending(t => t.ReleaseDate).ToListAsync(ct);
    }

    public async Task AddAsync(Track track, CancellationToken ct = default) =>
        await _db.Tracks.AddAsync(track, ct);
}
