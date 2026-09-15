namespace TrackManagement.Application.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<(string Token, DateTime ExpiresAtUtc)> CreateAsync(string username, CancellationToken cancellationToken = default);

    Task<(string Username, string Token, DateTime ExpiresAtUtc)?> RotateAsync(string token, CancellationToken cancellationToken = default);

    Task RevokeAsync(string token, CancellationToken cancellationToken = default);
}