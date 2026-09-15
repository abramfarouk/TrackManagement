using TrackManagement.Application.DTOs;

namespace TrackManagement.Application.Interfaces.Services;

public record AuthenticatedUser(string Username, string Role);

public record LoginAttemptResult(AuthenticatedUser? User, bool IsLocked, DateTime? LockedUntilUtc = null);

public interface IUserService
{
    Task<LoginAttemptResult> AuthenticateAsync(LoginRequestDto request, CancellationToken ct = default);

    Task<AuthenticatedUser?> GetByUsernameAsync(string username, CancellationToken ct = default);

    Task<AuthenticatedUser?> RegisterAsync(RegisterRequestDto request, bool isAdmin, CancellationToken ct = default);
}
