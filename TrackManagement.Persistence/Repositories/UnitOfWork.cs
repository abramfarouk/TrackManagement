using TrackManagement.Application.Interfaces.Persistence;
using TrackManagement.Application.Interfaces.Repositories;
using TrackManagement.Persistence.Context;

namespace TrackManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    
        private readonly TrackManagementDbContext _db;

        public UnitOfWork(
            TrackManagementDbContext db,
            IArtistRepository artists,
            ITrackRepository tracks,
            IDspRepository dsps)
        {
            _db = db;
            Artists = artists;
            Tracks = tracks;
            Dsps = dsps;
        }

        public IArtistRepository Artists { get; }

        public ITrackRepository Tracks { get; }

        public IDspRepository Dsps { get; }

        public Task<int> SaveChangesAsync(
            CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }
    }

