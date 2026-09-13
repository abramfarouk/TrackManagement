using TrackManagement.Application.DTOs;
using TrackManagement.Application.Exceptions;
using TrackManagement.Application.Interfaces.Persistence;
using TrackManagement.Application.Interfaces.Services;
using TrackManagement.Domain.Entities;

namespace TrackManagement.Infrastructure.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IUnitOfWork _uow;

        public ArtistService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ArtistDto>> GetAllAsync(CancellationToken ct = default)
        {
            var artists = await _uow.Artists.GetAllAsync(ct);

            return artists
                .Select(a => new ArtistDto(a.Id, a.Name, a.Email, a.Country, a.Tracks.Count))
                .ToList();
        }

        public async Task<ArtistDto> CreateAsync(CreateArtistDto dto, CancellationToken ct = default)
        {
            if (await _uow.Artists.EmailExistsAsync(dto.Email, ct))
            {
                throw new ConflictException($"An artist with email '{dto.Email}' already exists.");
            }

            var artist = new Artist
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant(),
                Country = dto.Country.Trim()
            };

            await _uow.Artists.AddAsync(artist, ct);
            await _uow.SaveChangesAsync(ct);

            return new ArtistDto(artist.Id, artist.Name, artist.Email, artist.Country, 0);
        }
    }

}
