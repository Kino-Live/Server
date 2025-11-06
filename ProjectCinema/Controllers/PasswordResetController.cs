using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.Interfaces;

namespace ProjectCinema.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly IPasswordResetService _passwordResetService;
        private readonly IValidator<PasswordResetRequestDTO> _passwordResetRequestValidator;
        private readonly IValidator<PasswordResetConfirmDTO> _passwordResetConfirmValidator;

        public PasswordResetController(
            IPasswordResetService passwordResetService,
            IValidator<PasswordResetRequestDTO> passwordResetRequestValidator,
            IValidator<PasswordResetConfirmDTO> passwordResetConfirmValidator)
        {
            _passwordResetService = passwordResetService;
            _passwordResetRequestValidator = passwordResetRequestValidator;
            _passwordResetConfirmValidator = passwordResetConfirmValidator;
        }

        [HttpPost("request")]
        public async Task<ActionResult> RequestPasswordResetAsync([FromBody] PasswordResetRequestDTO dto)
        {
            var validResult = await _passwordResetRequestValidator.ValidateAsync(dto);
            if (!validResult.IsValid)
            {
                return BadRequest(validResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            try
            {
                var requestIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();

                await _passwordResetService.RequestPasswordResetAsync(dto, requestIp, userAgent);

                return Accepted(new { message = "If the email exists, a password reset link has been sent." });
            }
            catch
            {
                return StatusCode(500, new { error = "An error occurred while processing your request." });
            }
        }

        [HttpGet("validate")]
        public async Task<ActionResult> ValidateTokenAsync([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest(new { error = "Token is required" });
            }

            try
            {
                var isValid = await _passwordResetService.ValidateTokenAsync(token);

                if (isValid)
                {
                    return Ok(new { valid = true, message = "Token is valid" });
                }

                return StatusCode(410, new { valid = false, message = "Token is invalid, expired, or already used" });
            }
            catch
            {
                return StatusCode(500, new { error = "An error occurred while validating the token." });
            }
        }

        [HttpPost("confirm")]
        public async Task<ActionResult> ConfirmPasswordResetAsync([FromBody] PasswordResetConfirmDTO dto)
        {
            var validResult = await _passwordResetConfirmValidator.ValidateAsync(dto);
            if (!validResult.IsValid)
            {
                return BadRequest(validResult.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            try
            {
                var result = await _passwordResetService.ConfirmPasswordResetAsync(dto);

                if (result)
                {
                    return Ok(new { message = "Password has been reset successfully" });
                }

                return StatusCode(410, new { error = "Token is invalid, expired, or already used" });
            }
            catch
            {
                return StatusCode(500, new { error = "An error occurred while resetting your password." });
            }
        }
    }
}


