namespace TrackManagement.Infrastructure.Auth;

public class LoginSecuritySettings
{
    public int MaxFailedAttempts { get; set; }

    public int LockoutMinutes { get; set; }
}
