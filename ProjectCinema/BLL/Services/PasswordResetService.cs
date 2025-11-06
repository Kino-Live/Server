using Microsoft.Extensions.Options;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Entities;
using ProjectCinema.Repositories.Interfaces;
using ProjectCinema.Settings;
using System.Security.Cryptography;
using System.Text;

namespace ProjectCinema.BLL.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IPasswordResetTokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly PasswordResetOptions _passwordResetOptions;

        public PasswordResetService(
            IPasswordResetTokenRepository tokenRepository,
            IUserRepository userRepository,
            IEmailSender emailSender,
            IOptions<PasswordResetOptions> passwordResetOptions)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _emailSender = emailSender;
            _passwordResetOptions = passwordResetOptions.Value;
        }

        public async Task RequestPasswordResetAsync(PasswordResetRequestDTO dto, string? requestIp, string? userAgent)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
            {
                return;
            }

            var tokenBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            var token = Convert.ToBase64String(tokenBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");

            var tokenHash = ComputeTokenHash(token);

            var expiresAt = DateTime.UtcNow.AddMinutes(_passwordResetOptions.TokenTTLMinutes);

            var resetToken = new PasswordResetToken
            {
                UserId = user.UserId,
                TokenHash = tokenHash,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow,
                UsedAt = null,
                RequestIp = requestIp,
                UserAgent = userAgent
            };

            await _tokenRepository.CreateAsync(resetToken);

            var resetUrl = BuildResetUrl(token, dto.RedirectUrl);

            try
            {
                var emailSubject = "Password Reset Request";
                var emailBody = BuildEmailBody(user.FirstName, resetUrl, _passwordResetOptions.TokenTTLMinutes);
                await _emailSender.SendAsync(user.Email, emailSubject, emailBody, isHtml: false);
            }
            catch
            {
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            var tokenHash = ComputeTokenHash(token);

            var resetToken = await _tokenRepository.FindByTokenHashAsync(tokenHash);
            if (resetToken == null)
            {
                return false;
            }

            if (resetToken.ExpiresAt < DateTime.UtcNow)
            {
                return false;
            }

            if (resetToken.UsedAt != null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ConfirmPasswordResetAsync(PasswordResetConfirmDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token))
            {
                return false;
            }

            var tokenHash = ComputeTokenHash(dto.Token);

            var resetToken = await _tokenRepository.FindByTokenHashAsync(tokenHash);
            if (resetToken == null)
            {
                return false;
            }

            if (resetToken.ExpiresAt < DateTime.UtcNow)
            {
                return false;
            }

            if (resetToken.UsedAt != null)
            {
                return false;
            }

            if (resetToken.User == null)
            {
                return false;
            }

            var user = resetToken.User;

            var (passwordHash, passwordSalt) = HashPassword(dto.NewPassword);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.UpdatedAt = DateTime.UtcNow;

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            await _tokenRepository.MarkAsUsedAsync(resetToken.PasswordResetTokenId);

            return true;
        }

        private string ComputeTokenHash(string token)
        {
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            var hashBytes = SHA256.HashData(tokenBytes);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        private (byte[] hash, byte[] salt) HashPassword(string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[passwordBytes.Length + salt.Length];
            Array.Copy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Array.Copy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            var hash = System.Security.Cryptography.SHA512.HashData(saltedPassword);

            return (hash, salt);
        }

        private string BuildResetUrl(string token, string? redirectUrl)
        {
            if (!string.IsNullOrWhiteSpace(redirectUrl))
            {
                if (IsValidRedirectUrl(redirectUrl))
                {
                    var separator = redirectUrl.Contains("?") ? "&" : "?";
                    return $"{redirectUrl}{separator}token={Uri.EscapeDataString(token)}";
                }
            }

            var defaultUrl = _passwordResetOptions.DefaultRedirectUrl;
            if (string.IsNullOrWhiteSpace(defaultUrl))
            {
                throw new InvalidOperationException("DefaultRedirectUrl is not configured");
            }

            var defaultSeparator = defaultUrl.Contains("?") ? "&" : "?";
            return $"{defaultUrl}{defaultSeparator}token={Uri.EscapeDataString(token)}";
        }

        private bool IsValidRedirectUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return false;
            }

            var host = uri.Host.ToLowerInvariant();
            var allowedHosts = _passwordResetOptions.AllowedRedirectHosts ?? Array.Empty<string>();

            return allowedHosts.Any(allowedHost =>
                host == allowedHost.ToLowerInvariant() ||
                host == $"www.{allowedHost.ToLowerInvariant()}");
        }

        private string BuildEmailBody(string firstName, string resetUrl, int ttlMinutes)
        {
            return $@"Hello {firstName},

You have requested to reset your password for your ProjectCinema account.

Click the link below to reset your password:
{resetUrl}

This link will expire in {ttlMinutes} minutes.

If you did not request a password reset, please ignore this email or contact support if you have concerns.

Best regards,
ProjectCinema Team";
        }
    }
}

