using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackManagement.Application.DTOs;
using TrackManagement.Application.Exceptions;
using TrackManagement.Application.Interfaces.Persistence;
using TrackManagement.Application.Interfaces.Services;
using TrackManagement.Domain.Entities;
using TrackManagement.Domain.Enums;

namespace TrackManagement.Infrastructure.Services
{
    public class TrackService : ITrackService
    {
        private readonly IUnitOfWork _uow;

        public TrackService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<TrackDto>> GetFilteredAsync(TrackFilterDto filter, CancellationToken ct = default)
        {
            if (!string.IsNullOrWhiteSpace(filter.Status) &&
                !Enum.TryParse<TrackStatus>(filter.Status, true, out _))
            {
                throw new ValidationAppException("status",
                    $"'{filter.Status}' is not a valid status. Valid values: Draft, Submitted, Distributed.");
            }

            var tracks = await _uow.Tracks.GetFilteredAsync(filter, ct);

            return tracks.Select(MapToTrackDto).ToList();
        }

        public async Task<TrackDetailDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var track = await _uow.Tracks.GetByIdWithDetailsAsync(id, ct)
                ?? throw new NotFoundException(nameof(Track), id);

            return MapToTrackDetailDto(track);
        }

        public async Task<TrackDto> CreateAsync(CreateTrackDto dto, CancellationToken ct = default)
        {
            var artist = await _uow.Artists.GetByIdAsync(dto.ArtistId, ct)
                ?? throw new ValidationAppException("artistId", $"Artist with id '{dto.ArtistId}' does not exist.");

            if (await _uow.Tracks.IsrcExistsAsync(dto.Isrc, ct))
            {
                throw new ConflictException($"A track with ISRC '{dto.Isrc}' already exists.");
            }

            var track = new Track
            {
                Title = dto.Title.Trim(),
                ArtistId = artist.Id,
                Isrc = dto.Isrc.Trim().ToUpperInvariant(),
                ReleaseDate = dto.ReleaseDate.Date,
                Genre = dto.Genre.Trim(),
                Status = TrackStatus.Draft
            };

            await _uow.Tracks.AddAsync(track, ct);
            await _uow.SaveChangesAsync(ct);

            return new TrackDto(track.Id, track.Title, artist.Id, artist.Name, track.Isrc,
                track.ReleaseDate, track.Genre, track.Status.ToString());
        }

        public async Task<TrackDetailDto> DistributeAsync(int trackId, DistributeTrackDto dto, CancellationToken ct = default)
        {
            var track = await _uow.Tracks.GetByIdWithDetailsAsync(trackId, ct)
                ?? throw new NotFoundException(nameof(Track), trackId);

            var requestedIds = dto.DspIds.Distinct().ToList();
            var dsps = await _uow.Dsps.GetByIdsAsync(requestedIds, ct);

            var missingIds = requestedIds.Except(dsps.Select(d => d.Id)).ToList();
            if (missingIds.Count > 0)
            {
                throw new ValidationAppException("dspIds",
                    $"The following DSP ids do not exist: {string.Join(", ", missingIds)}.");
            }

            var now = DateTime.UtcNow;

            foreach (var dsp in dsps)
            {
                var existing = track.TrackDistributions.FirstOrDefault(td => td.DspId == dsp.Id);
                if (existing is not null)
                {
                    existing.Status = DistributionStatus.Pending;
                    existing.SubmittedAt = now;
                    existing.UpdatedAtUtc = now;
                }
                else
                {
                    track.TrackDistributions.Add(new TrackDistribution
                    {
                        TrackId = track.Id,
                        DspId = dsp.Id,
                        SubmittedAt = now,
                        Status = DistributionStatus.Pending
                    });
                }
            }

            if (track.Status == TrackStatus.Draft)
            {
                track.Status = TrackStatus.Submitted;
            }

            track.UpdatedAtUtc = now;

            await _uow.SaveChangesAsync(ct);

            return MapToTrackDetailDto(track);
        }

        public async Task<TrackDto> UpdateStatusAsync(int trackId, UpdateTrackStatusDto dto, CancellationToken ct = default)
        {
            if (!Enum.TryParse<TrackStatus>(dto.Status, true, out var newStatus))
            {
                throw new ValidationAppException("status",
                    $"'{dto.Status}' is not a valid status. Valid values: Draft, Submitted, Distributed.");
            }

            var track = await _uow.Tracks.GetByIdWithDetailsAsync(trackId, ct)
                ?? throw new NotFoundException(nameof(Track), trackId);

            track.Status = newStatus;
            track.UpdatedAtUtc = DateTime.UtcNow;

            await _uow.SaveChangesAsync(ct);

            return MapToTrackDto(track);
        }

        private static TrackDto MapToTrackDto(Track track) => new(
            track.Id,
            track.Title,
            track.ArtistId,
            track.Artist?.Name ?? string.Empty,
            track.Isrc,
            track.ReleaseDate,
            track.Genre,
            track.Status.ToString());

        private static TrackDetailDto MapToTrackDetailDto(Track track) => new(
            track.Id,
            track.Title,
            track.ArtistId,
            track.Artist?.Name ?? string.Empty,
            track.Isrc,
            track.ReleaseDate,
            track.Genre,
            track.Status.ToString(),
            track.TrackDistributions
                .OrderBy(td => td.SubmittedAt)
                .Select(td => new TrackDistributionDto(
                    td.Id, td.DspId, td.Dsp?.Name ?? string.Empty, td.SubmittedAt, td.Status.ToString()))
                .ToList());
    }
}
