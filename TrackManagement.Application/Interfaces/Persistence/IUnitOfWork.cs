
using TrackManagement.Application.Interfaces.Repositories;

namespace TrackManagement.Application.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        IArtistRepository Artists { get; }

        ITrackRepository Tracks { get; }

        IDspRepository Dsps { get; }

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}
