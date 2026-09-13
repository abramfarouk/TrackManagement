using TrackManagement.Application.Interfaces.Persistence;
using TrackManagement.Application.Interfaces.Repositories;
using TrackManagement.Persistence.Context;

namespace TrackManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly TrackManagementDbContext _db;

    public UnitOfWork(IUnitOfWork db, IArtistRepository artists, ITrackRepository tracks, IDspRepository dsps)
    {
        Artists = artists;
        Tracks = tracks;
        Dsps = dsps;
        _db = db as TrackManagementDbContext ?? throw new ArgumentNullException(nameof(db));
    }

    public IArtistRepository Artists { get; }

    public ITrackRepository Tracks { get; }

    public IDspRepository Dsps { get; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
