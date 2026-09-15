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
        private readonly IConfiguration _configuration;

        public AuthController(IJwtTokenService jwtTokenService, IConfiguration configuration)
        {
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            var expectedUsername = _configuration["DemoUser:Username"];
            var expectedPassword = _configuration["DemoUser:Password"];

            if (request.Username != expectedUsername || request.Password != expectedPassword)
            {
                return Unauthorized(ApiResponse<bool>.Fail("Username or Password Invalid", StatusCodes.Status401Unauthorized));
            }

            var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(request.Username);

            return Ok(new LoginResponseDto(token, expiresAtUtc));
        }
    }
}
