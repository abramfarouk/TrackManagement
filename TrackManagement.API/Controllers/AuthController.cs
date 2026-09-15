using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrackManagement.API.Response;
using TrackManagement.Application.DTOs;
using TrackManagement.Application.Interfaces.Services;

namespace TrackManagement.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserService _userService;

        public AuthController(
            IJwtTokenService jwtTokenService,
            IRefreshTokenService refreshTokenService,
            IUserService userService)
        {
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
            _userService = userService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
        {
            var loginAttempt = await _userService.AuthenticateAsync(request, cancellationToken);

            if (loginAttempt.IsLocked)
            {
                var remainingMinutes = loginAttempt.LockedUntilUtc.HasValue
                    ? Math.Max(1, (int)Math.Ceiling((loginAttempt.LockedUntilUtc.Value - DateTime.UtcNow).TotalMinutes))
                    : 1;
                return StatusCode(StatusCodes.Status429TooManyRequests,
                    ApiResponse<bool>.Fail($"Too many failed login attempts. Try again in {remainingMinutes} minutes.", StatusCodes.Status429TooManyRequests));
            }

            if (loginAttempt.User is null)
            {
                return Unauthorized(ApiResponse<bool>.Fail("Username or Password Invalid", StatusCodes.Status401Unauthorized));
            }

            var user = loginAttempt.User;
            var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(user.Username, user.Role);
            var refreshToken = await _refreshTokenService.CreateAsync(user.Username, cancellationToken);
            SetRefreshCookie(refreshToken.Token, refreshToken.ExpiresAtUtc);

            return Ok(new LoginResponseDto(token, expiresAtUtc));
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthenticatedUser), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
        {
            var user = await _userService.RegisterAsync(
                request,
                User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"),
                cancellationToken);

            if (user is null)
            {
                return Forbid();
            }

            return Ok(user);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            if (!Request.Cookies.TryGetValue("trackmanagement.refresh", out var refreshToken))
            {
                return Unauthorized();
            }

            var rotated = await _refreshTokenService.RotateAsync(refreshToken, cancellationToken);
            if (rotated is null)
            {
                DeleteRefreshCookie();
                return Unauthorized();
            }

            var (username, replacementToken, expiresAtUtc) = rotated.Value;
            var user = await _userService.GetByUsernameAsync(username, cancellationToken);
            if (user is null)
            {
                DeleteRefreshCookie();
                return Unauthorized();
            }

            var (accessToken, accessTokenExpiresAtUtc) = _jwtTokenService.GenerateToken(user.Username, user.Role);
            SetRefreshCookie(replacementToken, expiresAtUtc);
            return Ok(new LoginResponseDto(accessToken, accessTokenExpiresAtUtc));
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            if (Request.Cookies.TryGetValue("trackmanagement.refresh", out var refreshToken))
            {
                await _refreshTokenService.RevokeAsync(refreshToken, cancellationToken);
            }

            DeleteRefreshCookie();
            return NoContent();
        }

        private void SetRefreshCookie(string token, DateTime expiresAtUtc)
        {
            Response.Cookies.Append("trackmanagement.refresh", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
                Expires = expiresAtUtc,
                Path = "/api/auth"
            });
        }

        private void DeleteRefreshCookie()
        {
            Response.Cookies.Delete("trackmanagement.refresh", new CookieOptions
            {
                Secure = Request.IsHttps,
                SameSite = Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
                Path = "/api/auth"
            });
        }
    }
}
