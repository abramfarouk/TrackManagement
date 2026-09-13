using System.ComponentModel.DataAnnotations;

namespace TrackManagement.Application.DTOs;

public record TrackDto(
    int Id,
    string Title,
    int ArtistId,
    string ArtistName,
    string Isrc,
    DateTime ReleaseDate,
    string Genre,
    string Status);

public record TrackDetailDto(
    int Id,
    string Title,
    int ArtistId,
    string ArtistName,
    string Isrc,
    DateTime ReleaseDate,
    string Genre,
    string Status,
    IReadOnlyList<TrackDistributionDto> Distributions);

public class CreateTrackDto
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(300)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "ArtistId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "ArtistId must be a positive integer.")]
    public int ArtistId { get; set; }

    [Required(ErrorMessage = "Isrc is required.")]
    [MaxLength(20)]
    public string Isrc { get; set; } = string.Empty;

    [Required(ErrorMessage = "ReleaseDate is required.")]
    public DateTime ReleaseDate { get; set; }

    [Required(ErrorMessage = "Genre is required.")]
    [MaxLength(100)]
    public string Genre { get; set; } = string.Empty;
}

public class DistributeTrackDto
{
    [Required(ErrorMessage = "At least one DspId is required.")]
    [MinLength(1, ErrorMessage = "At least one DspId is required.")]
    public List<int> DspIds { get; set; } = new();
}

public class UpdateTrackStatusDto
{
    [Required(ErrorMessage = "Status is required.")]
    public string Status { get; set; } = string.Empty;
}
