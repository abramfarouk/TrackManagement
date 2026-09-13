using Microsoft.EntityFrameworkCore;
using TrackManagement.Application.Interfaces;
using TrackManagement.Application.Interfaces.Repositories;
using TrackManagement.Domain.Entities;
using TrackManagement.Persistence.Context;

namespace TrackManagement.Persistence.Repositories;

public class DspRepository : IDspRepository
{
    private readonly TrackManagementDbContext _db;

    public DspRepository(TrackManagementDbContext db)
    {
        _db = db;
    }

    public Task<Dsp?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Dsps.FirstOrDefaultAsync(d => d.Id == id, ct);

    public Task<List<Dsp>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default) =>
        _db.Dsps.Where(d => ids.Contains(d.Id)).ToListAsync(ct);

    public Task<List<Dsp>> GetAllAsync(CancellationToken ct = default) =>
        _db.Dsps.OrderBy(d => d.Name).ToListAsync(ct);
}
