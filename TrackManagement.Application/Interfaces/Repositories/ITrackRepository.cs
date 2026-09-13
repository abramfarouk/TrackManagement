using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Application.DTOs;
using TrackManagement.Domain.Entities;

namespace TrackManagement.Application.Interfaces.Repositories
{
    public interface ITrackRepository
    {
        Task<Track?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<Track?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default);

        Task<bool> IsrcExistsAsync(string isrc, CancellationToken ct = default);

        Task<List<Track>> GetFilteredAsync(TrackFilterDto filter, CancellationToken ct = default);

        Task AddAsync(Track track, CancellationToken ct = default);
    }
}
