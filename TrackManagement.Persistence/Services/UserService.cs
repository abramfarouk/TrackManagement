using Microsoft.EntityFrameworkCore;
using TrackManagement.Application.DTOs;
using TrackManagement.Application.Exceptions;
using TrackManagement.Application.Interfaces.Services;
using TrackManagement.Domain.Entities;
using TrackManagement.Persistence.Context;

namespace TrackManagement.Persistence.Services;

public class UserService : IUserService
{
    private const int MaxFailedLoginAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Admin",
        "Operator",
        "Editor",
        "Viewer"
    };

    private readonly TrackManagementDbContext _dbContext;

    public UserService(TrackManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LoginAttemptResult> AuthenticateAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(item => item.Username == request.Username.Trim(), ct);

        if (user is null)
        {
            return new LoginAttemptResult(null, false);
        }

        var now = DateTime.UtcNow;
        if (user.LockedUntilUtc.HasValue && user.LockedUntilUtc > now)
        {
            return new LoginAttemptResult(null, true);
        }

        if (user.LockedUntilUtc.HasValue)
        {
            user.LockedUntilUtc = null;
            user.FailedLoginAttempts = 0;
        }

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= MaxFailedLoginAttempts)
            {
                user.LockedUntilUtc = now.Add(LockoutDuration);
            }

            await _dbContext.SaveChangesAsync(ct);
            return new LoginAttemptResult(null, user.LockedUntilUtc.HasValue);
        }

        user.FailedLoginAttempts = 0;
        user.LockedUntilUtc = null;
        await _dbContext.SaveChangesAsync(ct);
        return new LoginAttemptResult(new AuthenticatedUser(user.Username, user.Role), false);
    }

    public async Task<AuthenticatedUser?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .SingleOrDefaultAsync(item => item.Username == username, ct);

        return user is null ? null : new AuthenticatedUser(user.Username, user.Role);
    }

    public async Task<AuthenticatedUser?> RegisterAsync(RegisterRequestDto request, bool isAdmin, CancellationToken ct = default)
    {
        if (!isAdmin)
        {
            return null;
        }

        var username = request.Username.Trim();
        var role = request.Role.Trim();
        if (!AllowedRoles.Contains(role))
        {
            throw new ValidationAppException("role", "The selected role is not supported.");
        }

        role = AllowedRoles.First(item => item.Equals(role, StringComparison.OrdinalIgnoreCase));
        if (await _dbContext.Users.AnyAsync(item => item.Username == username, ct))
        {
            throw new ConflictException("Username is already in use");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = role
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(ct);

        return new AuthenticatedUser(user.Username, user.Role);
    }
}
