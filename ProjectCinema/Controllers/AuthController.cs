using Microsoft.AspNetCore.Mvc;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.Interfaces;

namespace ProjectCinema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTO>> RegisterAsync([FromBody] RegisterDTO registerDto)
        {
            try
            {
                var authResponse = await _authService.RegisterAsync(registerDto);
                return Ok(authResponse);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTO>> LoginAsync([FromBody] LoginUserDTO loginDto)
        {
            try
            {
                var authResponse = await _authService.LoginAsync(loginDto);
                return Ok(authResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDTO>> RefreshTokenAsync([FromBody] RefreshTokenDTO refreshTokenDto)
        {
            try
            {
                var authResponse = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
                return Ok(authResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<ActionResult> LogoutAsync()
        {
            try
            {
                // Extract token from Authorization header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { error = "Token not provided" });
                }

                // Get userId from token
                var userId = _jwtTokenService.GetUserIdFromToken(token);
                
                if (userId == null)
                {
                    return Unauthorized(new { error = "Invalid token" });
                }

                var result = await _authService.LogoutAsync(userId.Value);
                
                if (result)
                {
                    return Ok(new { message = "Logged out successfully" });
                }
                
                return BadRequest(new { error = "Logout failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}

