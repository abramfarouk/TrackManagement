using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TrackManagement.Application.Interfaces.Services;
using TrackManagement.Domain.Entities;
using TrackManagement.Persistence.Context;

namespace TrackManagement.Persistence.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly TrackManagementDbContext _dbContext;

    public RefreshTokenService(TrackManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(string Token, DateTime ExpiresAtUtc)> CreateAsync(
        string username,
        CancellationToken cancellationToken = default)
    {
        var token = GenerateToken();
        var expiresAtUtc = DateTime.UtcNow.AddDays(7);

        var storedToken = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(item => item.Username == username, cancellationToken);

        if (storedToken is null)
        {
            _dbContext.RefreshTokens.Add(new RefreshToken
            {
                Username = username,
                TokenHash = HashToken(token),
                ExpiresAtUtc = expiresAtUtc
            });
        }
        else
        {
            storedToken.TokenHash = HashToken(token);
            storedToken.ExpiresAtUtc = expiresAtUtc;
            storedToken.RevokedAtUtc = null;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (token, expiresAtUtc);
    }

    public async Task<(string Username, string Token, DateTime ExpiresAtUtc)?> RotateAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(token);
        var storedToken = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(item => item.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null || storedToken.RevokedAtUtc.HasValue || storedToken.ExpiresAtUtc <= DateTime.UtcNow)
        {
            return null;
        }

        storedToken.RevokedAtUtc = DateTime.UtcNow;
        var replacement = await CreateAsync(storedToken.Username, cancellationToken);
        return (storedToken.Username, replacement.Token, replacement.ExpiresAtUtc);
    }

    public async Task RevokeAsync(string token, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(token);
        var storedToken = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(item => item.TokenHash == tokenHash, cancellationToken);

        if (storedToken is null || storedToken.RevokedAtUtc.HasValue)
        {
            return;
        }

        storedToken.RevokedAtUtc = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private static string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }

    private static string HashToken(string token)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}