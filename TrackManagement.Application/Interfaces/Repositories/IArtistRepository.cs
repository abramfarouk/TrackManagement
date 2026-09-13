using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Domain.Entities;

namespace TrackManagement.Application.Interfaces.Repositories
{
    public interface IArtistRepository
    {
        Task<Artist?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<bool> ExistsAsync(int id, CancellationToken ct = default);

        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);

        Task<List<Artist>> GetAllAsync(CancellationToken ct = default);

        Task AddAsync(Artist artist, CancellationToken ct = default);
    }
}
