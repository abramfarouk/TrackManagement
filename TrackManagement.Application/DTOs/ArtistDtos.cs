using System.ComponentModel.DataAnnotations;

namespace TrackManagement.Application.DTOs;

public record ArtistDto(int Id, string Name, string Email, string Country, int TrackCount);

public class CreateArtistDto
{
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is not a valid email address.")]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Country is required.")]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;
}
