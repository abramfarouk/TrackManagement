using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Application.DTOs;

namespace TrackManagement.Application.Interfaces.Services
{

    public interface ITrackService
    {
        Task<List<TrackDto>> GetFilteredAsync(TrackFilterDto filter, CancellationToken ct = default);

        Task<TrackDetailDto> GetByIdAsync(int id, CancellationToken ct = default);

        Task<TrackDto> CreateAsync(CreateTrackDto dto, CancellationToken ct = default);

        Task<TrackDetailDto> DistributeAsync(int trackId, DistributeTrackDto dto, CancellationToken ct = default);

        Task<TrackDto> UpdateStatusAsync(int trackId, UpdateTrackStatusDto dto, CancellationToken ct = default);
    }

}
