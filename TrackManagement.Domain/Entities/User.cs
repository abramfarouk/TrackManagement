using TrackManagement.Domain.Common;

namespace TrackManagement.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public int FailedLoginAttempts { get; set; }

    public DateTime? LockedUntilUtc { get; set; }
}
