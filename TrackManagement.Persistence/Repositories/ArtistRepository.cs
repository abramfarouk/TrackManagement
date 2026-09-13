using Microsoft.EntityFrameworkCore;
using TrackManagement.Application.Interfaces.Repositories;
using TrackManagement.Domain.Entities;
using TrackManagement.Persistence.Context;

namespace TrackManagement.Persistence.Repositories;

public class ArtistRepository : IArtistRepository
{
    private readonly TrackManagementDbContext _db;

    public ArtistRepository(TrackManagementDbContext db)
    {
        _db = db;
    }

    public Task<Artist?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Artists.FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
        _db.Artists.AnyAsync(a => a.Id == id, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        _db.Artists.AnyAsync(a => a.Email.ToLower() == email.ToLower(), ct);

    public Task<List<Artist>> GetAllAsync(CancellationToken ct = default) =>
        _db.Artists
            .Include(a => a.Tracks)
            .OrderBy(a => a.Name)
            .ToListAsync(ct);

    public async Task AddAsync(Artist artist, CancellationToken ct = default) =>
        await _db.Artists.AddAsync(artist, ct);
}
