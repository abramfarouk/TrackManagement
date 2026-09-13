using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Application.DTOs;

namespace TrackManagement.Application.Interfaces.Services
{
    public interface IArtistService
    {
        Task<List<ArtistDto>> GetAllAsync(CancellationToken ct = default);

        Task<ArtistDto> CreateAsync(CreateArtistDto dto, CancellationToken ct = default);
    }
}
