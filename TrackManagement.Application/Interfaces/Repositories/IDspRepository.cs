using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Domain.Entities;

namespace TrackManagement.Application.Interfaces.Repositories
{
    public interface IDspRepository
    {
        Task<Dsp?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<List<Dsp>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);

        Task<List<Dsp>> GetAllAsync(CancellationToken ct = default);
    }
}
