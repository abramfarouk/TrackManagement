using TrackManagement.Domain.Common;

namespace TrackManagement.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Username { get; set; } = string.Empty;

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; }
}